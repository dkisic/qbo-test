using Abp;
using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Hangfire;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Exception;
using Intuit.Ipp.QueryFilter;
using Microsoft.EntityFrameworkCore;
using QBOTest.Configuration;
using QBOTest.QuickBooks.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using Partner = QBOTest.Partners.Partner;
using QBOCustomer = Intuit.Ipp.Data.Customer;
using QBOItem = Intuit.Ipp.Data.Item;
using Task = System.Threading.Tasks.Task;

namespace QBOTest.QuickBooks
{
    [AutomaticRetry(Attempts = 0)]
    public class QuickBooksSyncJob : AsyncBackgroundJob<QuickBooksSyncJobArgs>, ITransientDependency
    {
        private readonly IRepository<QuickBooksSyncLog, Guid> _quickBooksSyncLogRepository;
        private readonly IRepository<Partner, Guid> _partnerRepository;
        private readonly IRepository<Items.Item, Guid> _itemRepository;
        private readonly ISettingManager _settingManager;
        private readonly IQuickBooksApiManager _quickBooksApiManager;

        private int partnersSuccessfulSyncs = 0;
        private int partnersUnsuccessfulSyncs = 0;
        private int itemSuccessfulSyncs = 0;
        private int itemUnsuccessfulSyncs = 0;

        public QuickBooksSyncJob(
            IRepository<QuickBooksSyncLog, Guid> quickBooksSyncLogRepository,
            IRepository<Partner, Guid> partnerRepository,
            IRepository<Items.Item, Guid> itemRepository,
            ISettingManager settingManager,
            IQuickBooksApiManager quickBooksApiManager)
        {
            _quickBooksSyncLogRepository = quickBooksSyncLogRepository;
            _partnerRepository = partnerRepository;
            _itemRepository = itemRepository;
            _settingManager = settingManager;
            _quickBooksApiManager = quickBooksApiManager;
        }

        [UnitOfWork(isTransactional: false)]
        public override async Task ExecuteAsync(QuickBooksSyncJobArgs args)
        {
            try
            {
                var syncPartners = new Func<ServiceContext, UserIdentifier, Task>(SyncPartners);

                await _quickBooksApiManager.QBOApiCall(syncPartners, args.User);

                var syncItems = new Func<ServiceContext, UserIdentifier, Task>(SyncItems);

                await _quickBooksApiManager.QBOApiCall(syncItems, args.User);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        #region Partners

        public async Task SyncPartners(ServiceContext context, UserIdentifier user)
        {
            partnersSuccessfulSyncs = 0;
            partnersUnsuccessfulSyncs = 0;

            var customerService = new QueryService<QBOCustomer>(context);

            var dataService = new DataService(context);

            using (UnitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                var realmId = await _settingManager.GetSettingValueForTenantAsync(AppSettingNames.QuickBooksRealmId, user.TenantId.Value);

                var query = "Select * from Customer";

                var quickBooksPartners = customerService.ExecuteIdsQuery(query).ToList();

                foreach (var quickBooksPartner in quickBooksPartners)
                {
                    var partner = await _partnerRepository.FirstOrDefaultAsync(x => x.QuickBooksId == quickBooksPartner.Id && x.QuickBooksRealmId == realmId);

                    if (partner == null)
                        await MapPartnerFromQBOCustomer(partner, quickBooksPartner, user, realmId, true);
                    else
                    {
                        var mapPartnerFromQBO = false;

                        if (quickBooksPartner.MetaData.LastUpdatedTime != partner.QuickBooksLastUpdatedTime)
                        {
                            if (partner.QuickBooksLastUpdatedTime.HasValue)
                            {
                                if (quickBooksPartner.MetaData.LastUpdatedTime > partner.QuickBooksLastUpdatedTime)
                                    mapPartnerFromQBO = true;
                            }
                            else
                            {
                                if (quickBooksPartner.MetaData.LastUpdatedTime > partner.LastModificationTime.GetValueOrDefault(DateTime.Now.AddYears(-100)) && quickBooksPartner.MetaData.LastUpdatedTime > partner.CreationTime)
                                    mapPartnerFromQBO = true;
                            }

                            if (mapPartnerFromQBO)
                                await MapPartnerFromQBOCustomer(partner, quickBooksPartner, user, realmId);
                            else
                                await MapQBOCustomerFromPartner(dataService, partner, user, realmId, quickBooksPartner);
                        }
                    }
                }

                var existingPartners = await _partnerRepository
                    .GetAll()
                    .Where(x => string.IsNullOrEmpty(x.QuickBooksId) && !string.IsNullOrEmpty(x.Name))
                    .Take(20)
                    .ToListAsync();

                foreach (var partner in existingPartners)
                    if (!quickBooksPartners.Any(x => x.DisplayName == partner.FullName))
                        await MapQBOCustomerFromPartner(dataService, partner, user, realmId, null, true);
            }
        }

        private async Task MapPartnerFromQBOCustomer(Partner partner, QBOCustomer quickBooksPartner, UserIdentifier user, string realmId, bool create = false)
        {
            if (create)
                partner = new Partner();

            var addressParts = !string.IsNullOrEmpty(quickBooksPartner?.BillAddr?.Line1) ? quickBooksPartner?.BillAddr?.Line1.Split(new[] { ' ' }, 2).ToList() : new List<string>();

            partner.CreatorUserId = create ? user.UserId : partner.CreatorUserId;
            partner.Name = quickBooksPartner.GivenName ?? quickBooksPartner?.DisplayName;
            partner.FullName = quickBooksPartner.GivenName ?? quickBooksPartner?.DisplayName;

            if (create)
            {
                partner.CreationTime = DateTime.Now;
                partner.CreatorUserId = user.UserId;
                partner.TenantId = user.TenantId.Value;
            }
            else
            {
                partner.LastModificationTime = DateTime.Now;
                partner.LastModifierUserId = user.UserId;
            }

            //partner.Name = quickBooksPartner.CompanyName;
            //partner.CountryId = country.Id;
            //partner.IsCompany = !string.IsNullOrEmpty(quickBooksPartner.CompanyName);
            //partner.EmailAddress = quickBooksPartner?.PrimaryEmailAddr?.Address;
            //partner.PhoneNumber = quickBooksPartner?.PrimaryPhone?.FreeFormNumber ?? quickBooksPartner?.AlternatePhone?.FreeFormNumber;
            //partner.MobilePhoneNumber = quickBooksPartner?.Mobile?.FreeFormNumber;
            //partner.PostalCode = quickBooksPartner?.BillAddr?.PostalCode;
            //partner.City = quickBooksPartner?.BillAddr?.City;
            //partner.Province = quickBooksPartner?.BillAddr?.County;

            //if (addressParts.Any())
            //{
            //    partner.HouseNumber = addressParts[0];

            //    if (addressParts.Count > 1)
            //        partner.Street = addressParts[1];
            //}

            //partner.FullName = quickBooksPartner?.DisplayName;
            //partner.FullAddress = partner?.GetPartnerFullAddress();
            //partner.FullBilllingAddress = partner.DifferentBillingAddress ? partner.GetPartnerFullBillingAddress() : null;
            partner.QuickBooksRealmId = realmId;
            partner.QuickBooksId = quickBooksPartner.Id;
            partner.QuickBooksLastUpdatedTime = quickBooksPartner.MetaData.LastUpdatedTime;

            await _quickBooksSyncLogRepository.InsertAsync(new QuickBooksSyncLog
            {
                Id = Guid.NewGuid(),
                EntityName = partner.FullName ?? "-",
                SyncDirection = QuickBooksSyncDirection.FromQuickBooks,
                SyncEntity = QuickBooksSyncEntity.Partner,
                SyncStatus = QuickBooksSyncStatus.Synced,
                SyncAction = create ? QuickBooksSyncAction.Create : QuickBooksSyncAction.Update,
                CreationTime = DateTime.Now,
                CreatorUserId = user.UserId,
                TenantId = user.TenantId.Value
            });

            await CurrentUnitOfWork.SaveChangesAsync();

            if (create)
                await _partnerRepository.InsertAsync(partner);
            else
                await _partnerRepository.UpdateAsync(partner);

            partnersSuccessfulSyncs++;
        }

        private async Task MapQBOCustomerFromPartner(DataService dataService, Partner partner, UserIdentifier user, string realmId, QBOCustomer quickBooksPartner = null, bool create = false)
        {
            if (quickBooksPartner == null)
                quickBooksPartner = new QBOCustomer();

            //quickBooksPartner.GivenName = partner.FirstName;
            //quickBooksPartner.FamilyName = partner.LastName;
            //quickBooksPartner.CompanyName = partner.CompanyName;
            quickBooksPartner.MiddleName = partner.FullName ?? "-";
            quickBooksPartner.FullyQualifiedName = partner.FullName ?? "-";
            quickBooksPartner.ContactName = partner.FullName ?? "-";
            quickBooksPartner.GivenName = partner.FullName ?? "-";
            quickBooksPartner.FamilyName = partner.FullName ?? "-";
            quickBooksPartner.DisplayName = partner.FullName ?? "-";
            quickBooksPartner.CompanyName = partner.FullName ?? "-";
            //quickBooksPartner.PrimaryEmailAddr = new EmailAddress { Address = partner.EmailAddress };
            //quickBooksPartner.PrimaryPhone = new TelephoneNumber { FreeFormNumber = partner.PhoneNumber };
            //quickBooksPartner.Mobile = new TelephoneNumber { FreeFormNumber = partner.MobilePhoneNumber };
            //quickBooksPartner.BillAddr = new PhysicalAddress
            //{
            //    PostalCode = partner.PostalCode,
            //    City = partner.City,
            //    County = partner.Province,
            //    Line1 = partner.HouseNumber + " " + partner.Street,
            //};
            quickBooksPartner.Overview = partner.Id.ToString();
            quickBooksPartner.AltContactName = partner.Id.ToString();
            quickBooksPartner.AcctNum = partner.Id.ToString();
            quickBooksPartner.Notes = partner.Id.ToString();

            QBOCustomer response;

            try
            {
                if (create)
                    response = dataService.Add(quickBooksPartner);
                else
                    response = dataService.Update(quickBooksPartner);

                partner.QuickBooksId = response.Id;
                partner.QuickBooksLastUpdatedTime = response.MetaData.LastUpdatedTime;
                partner.QuickBooksRealmId = realmId;

                await _partnerRepository.UpdateAsync(partner);

                await _quickBooksSyncLogRepository.InsertAsync(new QuickBooksSyncLog
                {
                    Id = Guid.NewGuid(),
                    EntityName = partner.FullName,
                    SyncDirection = QuickBooksSyncDirection.ToQuickBooks,
                    SyncEntity = QuickBooksSyncEntity.Partner,
                    SyncStatus = QuickBooksSyncStatus.Synced,
                    SyncAction = create ? QuickBooksSyncAction.Create : QuickBooksSyncAction.Update,
                    CreatorUserId = user.UserId,
                    TenantId = user.TenantId.Value
                });

                await CurrentUnitOfWork.SaveChangesAsync();

                partnersSuccessfulSyncs++;
            }
            catch (IdsException exception)
            {
                partnersUnsuccessfulSyncs++;

                await _quickBooksSyncLogRepository.InsertAsync(new QuickBooksSyncLog
                {
                    Id = Guid.NewGuid(),
                    EntityName = partner.FullName ?? "-",
                    SyncDirection = QuickBooksSyncDirection.ToQuickBooks,
                    SyncEntity = QuickBooksSyncEntity.Partner,
                    SyncStatus = QuickBooksSyncStatus.Failed,
                    SyncAction = create ? QuickBooksSyncAction.Create : QuickBooksSyncAction.Update,
                    Description = exception.Message,
                    CreatorUserId = user.UserId,
                    TenantId = user.TenantId.Value
                });

                await CurrentUnitOfWork.SaveChangesAsync();
            }

        }

        #endregion

        #region Items

        public async Task SyncItems(ServiceContext context, UserIdentifier user)
        {
            var list = new List<QuickBooksSyncLog>();

            var itemService = new QueryService<QBOItem>(context);

            var accountItem = new QueryService<Account>(context);

            var dataService = new DataService(context);

            using (UnitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                var realmId = await _settingManager.GetSettingValueForTenantAsync(AppSettingNames.QuickBooksRealmId, user.TenantId.Value);

                var query = "Select * from Item";

                var quickBooksItems = itemService.ExecuteIdsQuery(query).ToList();

                var accountsQuery = "Select * from Account";

                var account = accountItem.ExecuteIdsQuery(accountsQuery).FirstOrDefault(x => x.AccountType == AccountTypeEnum.Income);

                foreach (var quickBooksItem in quickBooksItems)
                {
                    var item = await _itemRepository.FirstOrDefaultAsync(x =>
                    (x.QuickBooksId == quickBooksItem.Id && x.QuickBooksRealmId == realmId) ||
                    x.Name == quickBooksItem.Name);

                    if (item == null)
                        await MapItemFromQBOItem(item, quickBooksItem, user, realmId, true);
                    else
                    {
                        var mapItemFromQBO = false;

                        if (quickBooksItem.MetaData.LastUpdatedTime != item.QuickBooksLastUpdatedTime)
                        {
                            if (item.QuickBooksLastUpdatedTime.HasValue)
                            {
                                if (quickBooksItem.MetaData.LastUpdatedTime > item.QuickBooksLastUpdatedTime)
                                    mapItemFromQBO = true;
                            }
                            else
                            {
                                if (quickBooksItem.MetaData.LastUpdatedTime > item.LastModificationTime.GetValueOrDefault(DateTime.Now.AddYears(-100)) && quickBooksItem.MetaData.LastUpdatedTime > item.CreationTime)
                                    mapItemFromQBO = true;
                            }

                            if (mapItemFromQBO)
                                await MapItemFromQBOItem(item, quickBooksItem, user, realmId);
                            else
                                await MapQBOItemFromItem(dataService, item, account, user, realmId, quickBooksItem);
                        }
                    }
                }

                var existingItems = await _itemRepository
                    .GetAll()
                    .Where(x => string.IsNullOrEmpty(x.QuickBooksId) && !string.IsNullOrEmpty(x.Name))
                    .Take(20)
                    .ToListAsync();

                foreach (var item in existingItems)
                    await MapQBOItemFromItem(dataService, item, account, user, realmId, null, true);
            }
        }

        private async Task MapItemFromQBOItem(Items.Item item, QBOItem quickBooksItem, UserIdentifier user, string realmId, bool create = false)
        {
            if (create)
                item = new Items.Item();

            item.Name = quickBooksItem.Name;
            item.CreatorUserId = create ? user.UserId : item.CreatorUserId;
            item.Price = quickBooksItem.UnitPrice;
            item.Notes = quickBooksItem.Description;
            item.QuickBooksRealmId = realmId;
            item.QuickBooksId = quickBooksItem.Id;
            item.QuickBooksLastUpdatedTime = quickBooksItem.MetaData.LastUpdatedTime;

            if (create)
            {
                item.CreationTime = DateTime.Now;
                item.CreatorUserId = user.UserId;
                item.TenantId = user.TenantId.Value;
            }
            else
            {
                item.LastModificationTime = DateTime.Now;
                item.LastModifierUserId = user.UserId;
            }

            await _quickBooksSyncLogRepository.InsertAsync(new QuickBooksSyncLog
            {
                Id = Guid.NewGuid(),
                EntityName = item.Name ?? "-",
                SyncDirection = QuickBooksSyncDirection.FromQuickBooks,
                SyncEntity = QuickBooksSyncEntity.Item,
                SyncStatus = QuickBooksSyncStatus.Synced,
                SyncAction = create ? QuickBooksSyncAction.Create : QuickBooksSyncAction.Update,
                CreationTime = DateTime.Now,
                CreatorUserId = user.UserId,
                TenantId = user.TenantId.Value
            });

            await CurrentUnitOfWork.SaveChangesAsync();

            if (create)
                await _itemRepository.InsertAsync(item);
            else
                await _itemRepository.UpdateAsync(item);

            itemSuccessfulSyncs++;
        }

        private async Task MapQBOItemFromItem(DataService dataService, Items.Item item, Account account, UserIdentifier user, string realmId, QBOItem quickBooksItem = null, bool create = false)
        {
            if (quickBooksItem == null)
                quickBooksItem = new QBOItem();

            quickBooksItem.Name = item.Name ?? "-";
            quickBooksItem.Description = item.Notes;
            quickBooksItem.UnitPrice = new decimal(100.0);
            quickBooksItem.UnitPriceSpecified = true;
            quickBooksItem.Type = ItemTypeEnum.Inventory;
            quickBooksItem.TypeSpecified = true;
            quickBooksItem.ExpenseAccountRef = new ReferenceType { name = account.Name, Value = account.Id };
            quickBooksItem.IncomeAccountRef = new ReferenceType { name = account.Name, Value = account.Id };

            QBOItem response;

            try
            {
                if (create)
                    response = dataService.Add(quickBooksItem);
                else
                    response = dataService.Update(quickBooksItem);

                item.QuickBooksId = response.Id;
                item.QuickBooksLastUpdatedTime = response.MetaData.LastUpdatedTime;
                item.QuickBooksRealmId = realmId;

                await _itemRepository.UpdateAsync(item);

                await _quickBooksSyncLogRepository.InsertAsync(new QuickBooksSyncLog
                {
                    EntityName = item.Name,
                    SyncDirection = QuickBooksSyncDirection.ToQuickBooks,
                    SyncEntity = QuickBooksSyncEntity.Item,
                    SyncStatus = QuickBooksSyncStatus.Synced,
                    SyncAction = create ? QuickBooksSyncAction.Create : QuickBooksSyncAction.Update,
                    CreatorUserId = user.UserId,
                    TenantId = user.TenantId.Value
                });

                await CurrentUnitOfWork.SaveChangesAsync();

                itemSuccessfulSyncs++;
            }
            catch (IdsException exception)
            {
                await _quickBooksSyncLogRepository.InsertAsync(new QuickBooksSyncLog
                {
                    Id = Guid.NewGuid(),
                    EntityName = item.Name ?? "-",
                    SyncDirection = QuickBooksSyncDirection.ToQuickBooks,
                    SyncEntity = QuickBooksSyncEntity.Item,
                    SyncStatus = QuickBooksSyncStatus.Failed,
                    SyncAction = create ? QuickBooksSyncAction.Create : QuickBooksSyncAction.Update,
                    Description = exception.Message,
                    CreatorUserId = user.UserId,
                    TenantId = user.TenantId.Value
                });

                await CurrentUnitOfWork.SaveChangesAsync();

                itemUnsuccessfulSyncs++;
            }
        }

        #endregion
    }
}

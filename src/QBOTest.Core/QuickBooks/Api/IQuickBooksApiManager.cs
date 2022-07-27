using Abp;
using Abp.Domain.Services;
using Intuit.Ipp.Core;
using System;
using System.Threading.Tasks;

namespace QBOTest.QuickBooks.Api
{
    public interface IQuickBooksApiManager : IDomainService
    {
        Task QBOApiCall(Func<ServiceContext, UserIdentifier, Task> apiCallFunction, UserIdentifier user);
    }
}
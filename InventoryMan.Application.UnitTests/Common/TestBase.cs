using InventoryMan.Core.Interfaces;
using Moq;

namespace InventoryMan.Application.UnitTests.Common
{
    public abstract class TestBase
    {
        protected readonly Mock<IUnitOfWork> UnitOfWorkMock;

        protected TestBase()
        {
            UnitOfWorkMock = new Mock<IUnitOfWork>();
        }

        protected void ResetMocks()
        {
            UnitOfWorkMock.Reset();
        }
    }
}
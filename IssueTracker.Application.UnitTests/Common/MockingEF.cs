using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.UnitTests.Common
{
    internal static class MockingEF
    {
        internal static Mock<DbSet<TEntity>> CreateFakeDbSet<TEntity>(List<TEntity> mockData) where TEntity : class
        {
            var data = mockData.AsQueryable();

            var mockSet = new Mock<DbSet<TEntity>>();
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            return mockSet;
        }
    }
}

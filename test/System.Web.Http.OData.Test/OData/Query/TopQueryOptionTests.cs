﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Builder.TestModels;
using Microsoft.Data.OData;
using Microsoft.TestCommon;

namespace System.Web.Http.OData.Query
{
    public class TopQueryOptionTests
    {
        [Fact]
        public void ConstructorNullContextThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new TopQueryOption("1", null));
        }

        [Fact]
        public void ConstructorNullRawValueThrows()
        {
            var model = new ODataModelBuilder().Add_Customer_EntityType().Add_Customers_EntitySet().GetEdmModel();

            Assert.Throws<ArgumentException>(() =>
                new TopQueryOption(null, new ODataQueryContext(model, typeof(Customer), "Customers")));
        }

        [Fact]
        public void ConstructorEmptyRawValueThrows()
        {
            var model = new ODataModelBuilder().Add_Customer_EntityType().Add_Customers_EntitySet().GetEdmModel();

            Assert.Throws<ArgumentException>(() =>
                new TopQueryOption(string.Empty, new ODataQueryContext(model, typeof(Customer), "Customers")));
        }

        [Theory]
        [InlineData("2")]
        [InlineData("100")]
        [InlineData("0")]
        [InlineData("-1")]
        public void CanConstructValidFilterQuery(string topValue)
        {
            var model = new ODataModelBuilder().Add_Customer_EntityType().Add_Customers_EntitySet().GetEdmModel();
            var context = new ODataQueryContext(model, typeof(Customer), "Customers");
            var top = new TopQueryOption(topValue, context);

            Assert.Same(context, top.Context);
            Assert.Equal(topValue, top.RawValue);
        }

        [Theory]
        [InlineData("NotANumber")]
        [InlineData("''")]
        [InlineData(" ")]
        public void ApplyInvalidSkipQueryThrows(string topValue)
        {
            var model = new ODataModelBuilder().Add_Customer_EntityType().Add_Customers_EntitySet().GetEdmModel();
            var context = new ODataQueryContext(model, typeof(Customer), "Customers");
            var top = new TopQueryOption(topValue, context);

            Assert.Throws<ODataException>(() =>
                top.ApplyTo(ODataQueryOptionTest.Customers));
        }

        [Fact]
        public void CanApplyTop()
        {
            var model = new ODataModelBuilder().Add_Customer_EntityType().Add_Customers_EntitySet().GetServiceModel();
            var topOption = new TopQueryOption("1", new ODataQueryContext(model, typeof(Customer), "Customers"));

            var customers = (new List<Customer>{
                new Customer { CustomerId = 1, Name = "Andy" },
                new Customer { CustomerId = 2, Name = "Aaron" },
                new Customer { CustomerId = 3, Name = "Alex" }
            }).AsQueryable();

            var results = topOption.ApplyTo(customers).ToArray();
            Assert.Equal(1, results.Length);
            Assert.Equal(1, results[0].CustomerId);
        }

        [Fact]
        public void CanApplySkipTopOrderby()
        {
            var model = new ODataModelBuilder().Add_Customer_EntityType().Add_Customers_EntitySet().GetServiceModel();
            var context = new ODataQueryContext(model, typeof(Customer), "Customers");
            var orderbyOption = new OrderByQueryOption("Name", context);
            var skipOption = new SkipQueryOption("2", context);
            var topOption = new TopQueryOption("2", context);

            var customers = (new List<Customer>{
                new Customer { CustomerId = 1, Name = "Andy" },
                new Customer { CustomerId = 2, Name = "Aaron" },
                new Customer { CustomerId = 3, Name = "Alex" },
                new Customer { CustomerId = 4, Name = "Ace" },
                new Customer { CustomerId = 5, Name = "Abner" }
            }).AsQueryable();

            IQueryable queryable = orderbyOption.ApplyTo(customers);
            queryable = skipOption.ApplyTo(queryable);
            queryable = topOption.ApplyTo(queryable);
            var results = ((IQueryable<Customer>)queryable).ToArray();
            Assert.Equal(2, results.Length);
            Assert.Equal(4, results[0].CustomerId);
            Assert.Equal(3, results[1].CustomerId);
        }
    }
}

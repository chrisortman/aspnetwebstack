﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Builder.TestModels;
using Microsoft.Data.Edm;

namespace System.Web.Http.OData
{
    public static class UsefulBuilders
    {
        public static IEdmModel GetServiceModel(this ODataModelBuilder builder)
        {
            return EdmModelHelperMethods.BuildEdmModel("Default", "Container", builder.StructuralTypes, builder.EntitySets);
        }

        public static ODataModelBuilder Add_Address_ComplexType(this ODataModelBuilder builder)
        {
            var address = builder.ComplexType<Address>();
            address.Property(a => a.HouseNumber);
            address.Property(a => a.Street);
            address.Property(a => a.City);
            address.Property(a => a.State);
            return builder;
        }
        public static ODataModelBuilder Add_ZipCode_ComplexType(this ODataModelBuilder builder)
        {
            var zipCode = builder.ComplexType<ZipCode>();
            zipCode.Property(z => z.Part1);
            zipCode.Property(z => z.Part2).IsOptional();
            return builder;
        }
        public static ODataModelBuilder Add_Customer_EntityType(this ODataModelBuilder builder)
        {
            var customer = builder.Entity<Customer>();
            customer.HasKey(c => c.CustomerId);
            customer.Property(c => c.CustomerId);
            customer.Property(c => c.Name);
            customer.Property(c => c.Website);
            customer.Property(c => c.SharePrice);
            customer.Property(c => c.ShareSymbol);
            return builder;
        }
        public static ODataModelBuilder Add_Order_EntityType(this ODataModelBuilder builder)
        {
            var order = builder.Entity<Order>();
            order.HasKey(o => o.OrderId);
            order.Property(o => o.OrderDate);
            order.Property(o => o.Price);
            order.Property(o => o.OrderDate);
            order.Property(o => o.DeliveryDate);
            order.Ignore(o => o.Cost);
            return builder;
        }
        public static ODataModelBuilder Add_CustomerOrders_Relationship(this ODataModelBuilder builder)
        {
            builder.Entity<Customer>().HasMany(c => c.Orders);
            return builder;
        }
        public static ODataModelBuilder Add_OrderCustomer_Relationship(this ODataModelBuilder builder)
        {
            builder.Entity<Order>().HasRequired(o => o.Customer);
            return builder;
        }
        public static ODataModelBuilder Add_Customers_EntitySet(this ODataModelBuilder builder)
        {
            builder.Add_Customer_EntityType().EntitySet<Customer>("Customers");
            return builder;
        }
        public static ODataModelBuilder Add_Orders_EntitySet(this ODataModelBuilder builder)
        {
            builder.EntitySet<Order>("Orders");
            return builder;
        }
        public static ODataModelBuilder Add_CustomerOrders_Binding(this ODataModelBuilder builder)
        {
            builder.EntitySet<Customer>("Customers").HasManyBinding(c => c.Orders, "Orders");
            return builder;
        }
        public static ODataModelBuilder Add_OrderCustomer_Binding(this ODataModelBuilder builder)
        {
            builder.EntitySet<Order>("Orders").HasRequiredBinding(o => o.Customer, "Customer");
            return builder;
        }
    }
}

﻿using commercetools.Sdk.Domain;
using commercetools.Sdk.LinqToQueryPredicate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace commercetools.Sdk.HttpApi.Tests
{
    // move this to a different project
    public class QueryPredicateTests
    {

        // key = "c14"
        // key != "c14"
        // not(key = "c14")
        // parent(id = "13c4ee51-ff35-490f-8e43-349e39c34646") and parent(typeId = "category")
        // parent(id = "13c4ee51-ff35-490f-8e43-349e39c34646" and typeId = "category")
        // version < 30 or key = "c14"
        // version < 30 and key = "c14"
        // not(version < 30 and key = "c14")
        // key in ("c14", "c15")
        //QueryPredicate<Category> queryPredicate3 = new QueryPredicate<Category>(c => c.Name["en"] == "categoryName");
        //QueryPredicate<Category> queryPredicate2 = new QueryPredicate<Category>(c => c.Key.In("c14", "c15"));
        //QueryPredicate<Category> queryPredicate4 = new QueryPredicate<Category>(c => !c.Key.In("c14", "c15"));        

        [Fact]
        public void ExpressionStringEqual()
        {
            Expression<Func<Category, bool>> expression = c => c.Key == "c14";
            QueryPredicateExpressionVisitor queryPredicateExpressionVisitor = new QueryPredicateExpressionVisitor();
            string result = queryPredicateExpressionVisitor.ProcessExpression(expression);
            Assert.Equal("key = \"c14\"", result);
        }

        [Fact]
        public void ExpressionStringNotEqual()
        {
            Expression<Func<Category, bool>> expression = c => c.Key != "c14";
            QueryPredicateExpressionVisitor queryPredicateExpressionVisitor = new QueryPredicateExpressionVisitor();
            string result = queryPredicateExpressionVisitor.ProcessExpression(expression);
            Assert.Equal("key != \"c14\"", result);
        }

        [Fact]
        public void ExpressionNotStringEqual()
        {
            Expression<Func<Category, bool>> expression = c => !(c.Key == "c14");
            QueryPredicateExpressionVisitor queryPredicateExpressionVisitor = new QueryPredicateExpressionVisitor();
            string result = queryPredicateExpressionVisitor.ProcessExpression(expression);
            Assert.Equal("not(key = \"c14\")", result);
        }

        [Fact]
        public void ExpressionPropertyTwoLevelStringEqual()
        {
            Expression<Func<Category, bool>> expression = c => c.Parent.Id == "13c4ee51-ff35-490f-8e43-349e39c34646";
            QueryPredicateExpressionVisitor queryPredicateExpressionVisitor = new QueryPredicateExpressionVisitor();
            string result = queryPredicateExpressionVisitor.ProcessExpression(expression);
            Assert.Equal("parent(id = \"13c4ee51-ff35-490f-8e43-349e39c34646\")", result);
        }

        [Fact]
        public void ExpressionPropertyThreeLevelStringEqual()
        {
            Expression<Func<ProductCatalogData, bool>> expression = p => p.Current.MasterVariant.Key == "p15";
            QueryPredicateExpressionVisitor queryPredicateExpressionVisitor = new QueryPredicateExpressionVisitor();
            string result = queryPredicateExpressionVisitor.ProcessExpression(expression);
            Assert.Equal("current(masterVariant(key = \"p15\"))", result);
        }

        [Fact]
        public void ExpressionPropertyIntEqual()
        {
            Expression<Func<Category, bool>> expression = c => c.Version == 30;
            QueryPredicateExpressionVisitor queryPredicateExpressionVisitor = new QueryPredicateExpressionVisitor();
            string result = queryPredicateExpressionVisitor.ProcessExpression(expression);
            Assert.Equal("version = 30", result);
        }        

        [Fact]
        public void ExpressionPropertyIntLessThan()
        {
            Expression<Func<Category, bool>> expression = c => c.Version < 30;
            QueryPredicateExpressionVisitor queryPredicateExpressionVisitor = new QueryPredicateExpressionVisitor();
            string result = queryPredicateExpressionVisitor.ProcessExpression(expression);
            Assert.Equal("version < 30", result);
        }

        [Fact]
        public void ExpressionPropertyTwoLevelIntLessThan()
        {
            Expression<Func<ProductData, bool>> expression = p => p.MasterVariant.Id < 30;
            QueryPredicateExpressionVisitor queryPredicateExpressionVisitor = new QueryPredicateExpressionVisitor();
            string result = queryPredicateExpressionVisitor.ProcessExpression(expression);
            Assert.Equal("masterVariant(id < 30)", result);
        }

        [Fact]
        public void ExpressionPropertyInString()
        {
            Expression<Func<Category, bool>> expression = c => c.Key.In("c14", "c15");
            QueryPredicateExpressionVisitor queryPredicateExpressionVisitor = new QueryPredicateExpressionVisitor();
            string result = queryPredicateExpressionVisitor.ProcessExpression(expression);
            Assert.Equal("key in (\"c14\", \"c15\")", result);
        }

        [Fact]
        public void ExpressionPropertyContainsAllString()
        {
            Expression<Func<Customer, bool>> expression = c => c.ShippingAddressIds.ContainsAll("c14", "c15");
            QueryPredicateExpressionVisitor queryPredicateExpressionVisitor = new QueryPredicateExpressionVisitor();
            string result = queryPredicateExpressionVisitor.ProcessExpression(expression);
            Assert.Equal("shippingAddressIds contains all (\"c14\", \"c15\")", result);
        }

        [Fact]
        public void ExpressionPropertyDictionaryEqual()
        {
            Expression<Func<Category, bool>> expression = c => c.Name["en"] == "men";
            QueryPredicateExpressionVisitor queryPredicateExpressionVisitor = new QueryPredicateExpressionVisitor();
            string result = queryPredicateExpressionVisitor.ProcessExpression(expression);
            Assert.Equal("name(en = \"men\")", result);
        }

        [Fact]
        public void ExpressionPropertyThreeLevelDictionaryEqual()
        {
            Expression<Func<Product, bool>> expression = p => p.MasterData.Current.Slug["en"] == "product";
            QueryPredicateExpressionVisitor queryPredicateExpressionVisitor = new QueryPredicateExpressionVisitor();
            string result = queryPredicateExpressionVisitor.ProcessExpression(expression);
            Assert.Equal("masterData(current(slug(en = \"product\")))", result);
        }

        [Fact]
        public void ContainsAll()
        {
            List<string> list = new List<string>() { "a", "b", "c" };
            bool result = list.ContainsAll("a", "b", "c");
            Assert.True(result);
        }
    }      
}

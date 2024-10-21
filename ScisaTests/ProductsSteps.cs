using TechTalk.SpecFlow;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ScisaApi.Data;
using ScisaApi.Models;
using System.Collections.Generic;
using System;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NuGet.ContentModel;
using Microsoft.Build.Logging;
using System.Net.Http.Json;
using ScisaApi.DTOs;

[Binding]
public class ProductsSteps
{
    private readonly HttpClient _httpClient;


    public ProductsSteps()
    {
        _httpClient = new HttpClient { BaseAddress = new System.Uri("https://localhost:7145/") };
    }

    [Given(@"User can see products")]
    public async Task GivenThereAreproducts()
    {
        HttpResponseMessage _response = await _httpClient.GetAsync("api/products");
        _response.EnsureSuccessStatusCode();
        var products = await _response.Content.ReadFromJsonAsync<List<RetrieveProduct>>();
        Assert.NotNull(products);

        _response = await _httpClient.GetAsync("api/categories");
        _response.EnsureSuccessStatusCode();
        var categories = await _response.Content.ReadFromJsonAsync<List<RetrieveCategory>>();
        Assert.NotNull(categories);

        var category = categories.FirstOrDefault();
        Assert.NotNull(category);

        if (products.Count == 0)
        {
            var product = new CreateProduct { Name = "Test List", Description="Test", CategoryIds = [category.Id] };

            _response = await _httpClient.PostAsJsonAsync("api/products", product);
            _response.EnsureSuccessStatusCode();
            _response = await _httpClient.GetAsync("api/products");
            _response.EnsureSuccessStatusCode();

            products = await _response.Content.ReadFromJsonAsync<List<RetrieveProduct>>();
        }
        Assert.NotNull(products);
        Assert.NotEmpty(products);
    }

    [Then(@"User can create a new product (.*)")]
    public async Task ThenUserCanCreateANewProduct(string productName)
    {
        HttpResponseMessage _response = await _httpClient.GetAsync("api/products");
        _response.EnsureSuccessStatusCode();
        var products = await _response.Content.ReadFromJsonAsync<List<RetrieveProduct>>();
        Assert.NotNull(products);

        if (products.Count > 0)
        {
            var productExist = products.Where(c => c.Name == productName).FirstOrDefault();
            Assert.NotNull(productExist);
            _response = await _httpClient.DeleteAsync($"api/products/{productExist.Id}");
            _response.EnsureSuccessStatusCode();
        }

        _response = await _httpClient.GetAsync("api/categories");
        _response.EnsureSuccessStatusCode();
        var categories = await _response.Content.ReadFromJsonAsync<List<RetrieveCategory>>();
        Assert.NotNull(categories);
        var category = categories.FirstOrDefault();
        Assert.NotNull(category);

        var product = new CreateProduct { Name = productName, Description="Test", CategoryIds = [category.Id] };

        _response = await _httpClient.PostAsJsonAsync("api/products", product);
        _response.EnsureSuccessStatusCode();

    }

    [Given(@"User can create a new product (.*)")]
    public async Task GivenUserCanCreateANewProduct(string productName)
    {
        HttpResponseMessage _response = await _httpClient.GetAsync("api/products");
        _response.EnsureSuccessStatusCode();
        var products = await _response.Content.ReadFromJsonAsync<List<RetrieveProduct>>();
        Assert.NotNull(products);

        if (products.Count > 0)
        {
            var productExist = products.Where(c => c.Name == productName).FirstOrDefault();
            if (productExist != null)
            {
                _response = await _httpClient.DeleteAsync($"api/products/{productExist.Id}");
                _response.EnsureSuccessStatusCode();
            }
        }

        _response = await _httpClient.GetAsync("api/categories");
        _response.EnsureSuccessStatusCode();
        var categories = await _response.Content.ReadFromJsonAsync<List<RetrieveCategory>>();
        Assert.NotNull(categories);
        var category = categories.FirstOrDefault();
        Assert.NotNull(category);

        var product = new CreateProduct { Name = productName, Description = "Test", CategoryIds = [category.Id] };

        _response = await _httpClient.PostAsJsonAsync("api/products", product);
        _response.EnsureSuccessStatusCode();

    }

    [Then(@"User can edit a product (.*)")]
    public async Task ThenUserCanEditAProduct(string productName)
    {
        HttpResponseMessage _response = await _httpClient.GetAsync("api/products");
        _response.EnsureSuccessStatusCode();
        var products = await _response.Content.ReadFromJsonAsync<List<RetrieveProduct>>();

        Assert.NotNull(products);

        var productEdit = products.Where(c => c.Name == productName).FirstOrDefault();
        Assert.NotNull(productEdit);
        var categoryIds = new List<int>();
        foreach (var categoryItem in productEdit.Categories)
        {
            categoryIds.Add(categoryItem.Id);
        }
        var product = new CreateProduct { Name = productName + "-Edited", Description="Test", CategoryIds = categoryIds };

        _response = await _httpClient.PutAsJsonAsync($"api/products/{productEdit.Id}", product);
        _response.EnsureSuccessStatusCode();

        product = new CreateProduct { Name = productName, Description = "Test", CategoryIds = categoryIds };
        Assert.NotEmpty(product.CategoryIds);
        _response = await _httpClient.PutAsJsonAsync($"api/products/{productEdit.Id}", product);
        _response.EnsureSuccessStatusCode();
    }

    [Then(@"User can detelete a product (.*)")]
    public async Task ThenUserCanDeleteAProduct(string productName)
    {
        HttpResponseMessage _response = await _httpClient.GetAsync("api/products");
        _response.EnsureSuccessStatusCode();
        var products = await _response.Content.ReadFromJsonAsync<List<RetrieveProduct>>();

        Assert.NotNull(products);
        if (products.Count > 0)
        {
            var productExist = products.Where(c => c.Name == productName).FirstOrDefault();
            if (productExist == null)
            {
                _response = await _httpClient.GetAsync("api/categories");
                _response.EnsureSuccessStatusCode();
                var categories = await _response.Content.ReadFromJsonAsync<List<RetrieveCategory>>();
                Assert.NotNull(categories);

                var category = categories.FirstOrDefault();
                Assert.NotNull(category);

                var product = new CreateProduct { Name = productName, Description="Test", CategoryIds = [category.Id] };
                _response = await _httpClient.PostAsJsonAsync("api/products", product);
                _response.EnsureSuccessStatusCode();
            }
        }

        _response = await _httpClient.GetAsync("api/products");
        _response.EnsureSuccessStatusCode();
        products = await _response.Content.ReadFromJsonAsync<List<RetrieveProduct>>();

        Assert.NotNull(products);

        var productDelete = products.Where(c => c.Name == productName).FirstOrDefault();

        Assert.NotNull(productDelete);

        _response = await _httpClient.DeleteAsync($"api/products/{productDelete.Id}");
        _response.EnsureSuccessStatusCode();
    }
}

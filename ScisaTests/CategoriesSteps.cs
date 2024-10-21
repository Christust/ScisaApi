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
public class CategoriesSteps
{
    private readonly HttpClient _httpClient;


    public CategoriesSteps()
    {
        _httpClient = new HttpClient { BaseAddress = new System.Uri("https://localhost:7145/") };
    }

    [Given(@"User can see categories")]
    public async Task GivenThereAreCategories()
    {
        HttpResponseMessage _response = await _httpClient.GetAsync("api/categories");
        _response.EnsureSuccessStatusCode();

        var categories = await _response.Content.ReadFromJsonAsync<List<Category>>();
        Assert.NotNull(categories);
        if (categories.Count == 0)
        {
            var category = new Category { Name = "Test List" };

            _response = await _httpClient.PostAsJsonAsync("api/categories", category);
            _response.EnsureSuccessStatusCode();
            _response = await _httpClient.GetAsync("api/categories");
            _response.EnsureSuccessStatusCode();

            categories = await _response.Content.ReadFromJsonAsync<List<Category>>();
        }
        Assert.NotNull(categories);
        Assert.NotEmpty(categories);
    }

    [Then(@"User can create a new category (.*)")]
    public async Task ThenUserCanCreateANewCategory(string categoryName)
    {
        HttpResponseMessage _response = await _httpClient.GetAsync("api/categories");
        _response.EnsureSuccessStatusCode();

        var categories = await _response.Content.ReadFromJsonAsync<List<Category>>();
        if (categories != null && categories.Count > 0)
        {
            var categoryExist = categories.Where(c => c.Name == categoryName).FirstOrDefault();
            if (categoryExist != null)
            {
                _response = await _httpClient.DeleteAsync($"api/categories/{categoryExist.Id}");
                _response.EnsureSuccessStatusCode();
            }
        }

        var category = new Category { Name = categoryName };

        _response = await _httpClient.PostAsJsonAsync("api/categories", category);
        _response.EnsureSuccessStatusCode();

    }

    [Given(@"User try create a category named (.*)")]
    public async Task GivenUserTryCreateACategoryNamed(string categoryName)
    {
        var category = new Category { Name = categoryName };

        HttpResponseMessage _response = await _httpClient.PostAsJsonAsync("api/categories", category);
        Assert.Equal(409, (double)_response.StatusCode);
    }

    [Then(@"The category (.*) was not saved")]
    public async Task ThenTheCategoryWasNotSaved(string categoryName)
    {
        HttpResponseMessage _response = await _httpClient.GetAsync("api/categories");
        _response.EnsureSuccessStatusCode();

        var categories = await _response.Content.ReadFromJsonAsync<List<Category>>();
        Assert.NotNull(categories);
        Assert.NotEmpty(categories);
        var categoryExist = categories.Where(c => c.Name == categoryName).ToList();
        Assert.Single(categoryExist);

    }

    [Given(@"User can create a new category (.*)")]
    public async Task GivenUserCanCreateANewCategory(string categoryName)
    {
        HttpResponseMessage _response = await _httpClient.GetAsync("api/categories");
        _response.EnsureSuccessStatusCode();

        var categories = await _response.Content.ReadFromJsonAsync<List<Category>>();
        Assert.NotNull(categories);
        if (categories.Count > 0)
        {
            var categoryExist = categories.Where(c => c.Name == categoryName).FirstOrDefault();
            if (categoryExist != null)
            {
                _response = await _httpClient.DeleteAsync($"api/categories/{categoryExist.Id}");
                _response.EnsureSuccessStatusCode();
            }
        }

        var category = new Category { Name = categoryName };

        _response = await _httpClient.PostAsJsonAsync("api/categories", category);
        _response.EnsureSuccessStatusCode();

    }

    [Then(@"User can edit a category (.*)")]
    public async Task ThenUserCanEditACategory(string categoryName)
    {
        var category = new Category { Name = categoryName };

        HttpResponseMessage _response = await _httpClient.GetAsync("api/categories");
        _response.EnsureSuccessStatusCode();
        var categories = await _response.Content.ReadFromJsonAsync<List<Category>>();

        Assert.NotNull(categories);
        if (categories.Count > 0)
        {
            var categoryExist = categories.Where(c => c.Name == categoryName).FirstOrDefault();
            if (categoryExist == null)
            {
                _response = await _httpClient.PostAsJsonAsync("api/categories", category);
                _response.EnsureSuccessStatusCode();
            }
        } else
        {
            _response = await _httpClient.PostAsJsonAsync("api/categories", category);
            _response.EnsureSuccessStatusCode();
        }

        _response = await _httpClient.GetAsync("api/categories");
        _response.EnsureSuccessStatusCode();
        categories = await _response.Content.ReadFromJsonAsync<List<Category>>();

        Assert.NotNull(categories);

        var categoryEdit = categories.Where(c => c.Name == categoryName).FirstOrDefault();
        Assert.NotNull(categoryEdit);
        category = new Category { Name = categoryName + "-Edited" };

        _response = await _httpClient.PutAsJsonAsync($"api/categories/{categoryEdit.Id}", category);
        _response.EnsureSuccessStatusCode();

        category = new Category { Name = categoryName };

        _response = await _httpClient.PutAsJsonAsync($"api/categories/{categoryEdit.Id}", category);
        _response.EnsureSuccessStatusCode();
    }

    [Then(@"User try edit a category with empty name")]
    public async Task ThenUserTryEditACategoryWithEmptyName()
    {
        var category = new Category { Name = "Test List" };

        HttpResponseMessage _response = await _httpClient.GetAsync("api/categories");
        _response.EnsureSuccessStatusCode();
        var categories = await _response.Content.ReadFromJsonAsync<List<Category>>();
        Assert.NotNull(categories);
        if (categories.Count > 0)
        {
            var categoryExist = categories.Where(c => c.Name == "Test List").FirstOrDefault();
            if (categoryExist == null)
            {
                _response = await _httpClient.PostAsJsonAsync("api/categories", category);
                _response.EnsureSuccessStatusCode();
            }
        }
        else
        {
            _response = await _httpClient.PostAsJsonAsync("api/categories", category);
            _response.EnsureSuccessStatusCode();
        }

        _response = await _httpClient.GetAsync("api/categories");
        _response.EnsureSuccessStatusCode();
        categories = await _response.Content.ReadFromJsonAsync<List<Category>>();
        Assert.NotNull(categories);
        var categoryEdit = categories.Where(c => c.Name == "Test List").FirstOrDefault();
        Assert.NotNull(categoryEdit);
        category = new Category { Name = "" };

        _response = await _httpClient.PutAsJsonAsync($"api/categories/{categoryEdit.Id}", category);
        Assert.Equal(400, (double)_response.StatusCode);
    }

    [Then(@"User can detelete a category (.*)")]
    public async Task ThenUserCanDeleteACategory(string categoryName)
    {
        HttpResponseMessage _response = await _httpClient.GetAsync("api/categories");
        _response.EnsureSuccessStatusCode();
        var categories = await _response.Content.ReadFromJsonAsync<List<Category>>();

        Assert.NotNull(categories);
        if (categories.Count > 0)
        {
            var categoryExist = categories.Where(c => c.Name == categoryName).FirstOrDefault();
            if (categoryExist == null)
            {
                var category = new Category { Name = categoryName };
                _response = await _httpClient.PostAsJsonAsync("api/categories", category);
                _response.EnsureSuccessStatusCode();
            }
        }

        _response = await _httpClient.GetAsync("api/categories");
        _response.EnsureSuccessStatusCode();
        categories = await _response.Content.ReadFromJsonAsync<List<Category>>();

        Assert.NotNull(categories);

        var categoryDelete = categories.Where(c => c.Name == categoryName).FirstOrDefault();

        Assert.NotNull(categoryDelete);

        _response = await _httpClient.DeleteAsync($"api/categories/{categoryDelete.Id}");
        _response.EnsureSuccessStatusCode();
    }

    [Given(@"User create a category (.*) with products")]
    public async Task GivenUserCanCreateACategoryWithProducts(string categoryName)
    {
        HttpResponseMessage _response = await _httpClient.GetAsync("api/categories");
        _response.EnsureSuccessStatusCode();
        var categories = await _response.Content.ReadFromJsonAsync<List<RetrieveCategory>>();
        Assert.NotNull(categories);

        if (categories.Count > 0)
        {
            var categoryExist = categories.Where(c => c.Name == categoryName).FirstOrDefault();

            if (categoryExist != null)
            {
                if (categoryExist.Products.Count > 0) {
                    foreach (var productItem in categoryExist.Products)
                    {
                        _response = await _httpClient.DeleteAsync($"api/products/{productItem.Id}");
                    }
                    
                }
                _response = await _httpClient.DeleteAsync($"api/categories/{categoryExist.Id}");
                _response.EnsureSuccessStatusCode();
            }
        }

        var category = new Category { Name = categoryName };
        _response = await _httpClient.PostAsJsonAsync("api/categories", category);
        _response.EnsureSuccessStatusCode();

        _response = await _httpClient.GetAsync("api/categories");
        _response.EnsureSuccessStatusCode();
        categories = await _response.Content.ReadFromJsonAsync<List<RetrieveCategory>>();
        Assert.NotNull(categories);

        var categoryProduct = categories.Where(c => c.Name == categoryName).First();
        Assert.NotNull(categoryProduct);

        var product = new CreateProduct { Name = categoryName, Description="Test", CategoryIds = [categoryProduct.Id] };

        _response = await _httpClient.PostAsJsonAsync("api/products", product);
        Assert.Equal(201, (double)_response.StatusCode);
    }

    [Then(@"User can not delete the category (.*)")]
    public async Task ThenUserCanNotDeleteTheCategory(string categoryName)
    {
        HttpResponseMessage _response = await _httpClient.GetAsync("api/categories");
        _response.EnsureSuccessStatusCode();
        var categories = await _response.Content.ReadFromJsonAsync<List<Category>>();
        Assert.NotNull(categories);

        _response = await _httpClient.GetAsync("api/categories");
        _response.EnsureSuccessStatusCode();
        categories = await _response.Content.ReadFromJsonAsync<List<Category>>();
        Assert.NotNull(categories);

        var categoryDelete = categories.Where(c => c.Name == categoryName).FirstOrDefault();
        Assert.NotNull(categoryDelete);

        _response = await _httpClient.DeleteAsync($"api/categories/{categoryDelete.Id}");
        Assert.Equal(409, (double)_response.StatusCode);
    }


}

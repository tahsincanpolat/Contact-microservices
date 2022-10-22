using Users.DataAccess;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Users.Models;

namespace Users.Test
{
    public class UserTest : WebApplicationFactory<User>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var root = new InMemoryDatabaseRoot();

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<UserDBContext>));
                services.AddDbContext<UserDBContext>(options =>
                    options.UseInMemoryDatabase("UserTest", root));
            });

            return base.CreateHost(builder);
        }

        [Fact]
        public async Task GetAllUsersTest()
        {
            await using var application = new UserTest();

            var client = application.CreateClient();
            var users = await client.GetFromJsonAsync<List<User>>("/users");
            
            Assert.Empty(users);
        }

        [Fact]
        public async Task GetSingleUsersTest()
        {
            await using var application = new UserTest();
            string id = "3fa85f64-5717-4563-b3fc-2c963f66afa6";
            var client = application.CreateClient();
            var user = await client.GetFromJsonAsync<List<User>>($"/users/{id}");

            Assert.Empty(user);
        }

        [Fact]
        public async Task GetAllUsersDetailsTest()
        {
            await using var application = new UserTest();

            var client = application.CreateClient();
            var usersDetails = await client.GetFromJsonAsync<List<User>>("/usersDetails");

            Assert.Empty(usersDetails);

        }
    }
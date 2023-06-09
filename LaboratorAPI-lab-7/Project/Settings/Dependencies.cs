﻿using DataLayer.Repositories;
using Core.Services;
using DataLayer;
using Microsoft.EntityFrameworkCore;

namespace Project.Settings
{
    public static class Dependencies
    {

        public static void Inject(WebApplicationBuilder applicationBuilder)
        {
            applicationBuilder.Services.AddControllers();
            applicationBuilder.Services.AddSwaggerGen();

            applicationBuilder.Services.AddDbContext<AppDbContext>();

            AddRepositories(applicationBuilder.Services);
            AddServices(applicationBuilder.Services);
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddScoped<StudentService>();
            services.AddScoped<AuthorizationService>();
            services.AddScoped<ClassService>();
            services.AddScoped<UserService>();
            services.AddScoped<RoleService>();
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<StudentsRepository>();
            services.AddScoped<ClassRepository>();
            services.AddScoped<UnitOfWork>();
            services.AddScoped<UsersRepository>();
            services.AddScoped<RolesRepository>();
        }

    }
}

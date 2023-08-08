// ***********************************************************************
// Assembly         : EToolsSecurity
// Author           : James Thompson
// Created          : 12-03-2022
//
// Last Modified By : James Thompson
// Last Modified On : 12-03-2022
// ***********************************************************************
// <copyright file="SecurityDependencies.cs" company="EToolsSecurity">
//     Copyright (c) NAIT. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using EToolsSecurity.BLL;
using EToolsSecurity.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SecurityDependencies
{
    /// <summary>
    /// Class SecurityDependencies.
    /// </summary>
    public static class SecurityDependencies
    {
        /// <summary>
        /// Securities the system backend dependencies.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="options">The options.</param>
        public static void SecuritySystemBackendDependencies(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<eTools2021Context>(options);

            services.AddTransient<SecurityService>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<eTools2021Context>();
                return new SecurityService(context);
            });
        }
    }
}
// ***********************************************************************
// Assembly         : EToolsSecurity
// Author           : James Thompson
// Created          : 12-03-2022
//
// Last Modified By : James Thompson
// Last Modified On : 12-03-2022
// ***********************************************************************
// <copyright file="EmployeeInfo.cs" company="EToolsSecurity">
//     Copyright (c) NAIT. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
#nullable disable
namespace EToolsSecurity.ViewModel
{
    /// <summary>
    /// Class EmployeeInfo.
    /// </summary>
    public class EmployeeInfo
    {
        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        /// <value>The employee identifier.</value>
        public int EmployeeID { get; set; }
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName { get; set; }
        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is manager.
        /// </summary>
        /// <value><c>true</c> if this instance is manager; otherwise, <c>false</c>.</value>
        public bool IsManager { get; set; }
    }
}

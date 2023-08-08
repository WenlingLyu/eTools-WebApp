// ***********************************************************************
// Assembly         : EToolsSecurity
// Author           : James Thompson
// Created          : 12-03-2022
//
// Last Modified By : James Thompson
// Last Modified On : 12-03-2022
// ***********************************************************************
// <copyright file="SecurityService.cs" company="EToolsSecurity">
//     Copyright (c) NAIT. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
#nullable disable
using EToolsSecurity.DAL;
using EToolsSecurity.ViewModel;

namespace EToolsSecurity.BLL
{
    /// <summary>
    /// Class SecurityService.
    /// </summary>
    public class SecurityService
    {
        #region Constructor and Context Dependency

        //  rename the context to match your system
        /// <summary>
        /// The context
        /// </summary>
        private readonly eTools2021Context _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        internal SecurityService(eTools2021Context context)
        {
            _context = context;
        }
        #endregion

        #region Services


        //  You will need to set up your extended method/backend dependencies

        //  Query to obtain the employee data
        /// <summary>
        /// Gets the employee information.
        /// </summary>
        /// <param name="isManager">if set to <c>true</c> [is manager].</param>
        /// <returns>EmployeeInfo.</returns>
        public EmployeeInfo GetEmployeeInfo(bool isManager)
        {
            int employeeId = isManager ? 3 : 1;
            return _context.Employees
                .Where(x => x.EmployeeID == employeeId)
                .Select(x => new EmployeeInfo()
                {
                    EmployeeID = x.EmployeeID,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    IsManager = isManager
                })
                .SingleOrDefault();
        }

        public EmployeeInfo GetEmployeeInfoSales(string loginID, string phone)
        {
            EmployeeInfo employeeInfo = null;
            List<Exception> errorList = new List<Exception>();

            var employeeExists = _context.Employees
                                    .Where(x => x.LoginID == loginID && x.ContactPhone == phone)
                                    .Select(x => x)
                                    .SingleOrDefault();

            if (employeeExists == null)
            {
                errorList.Add(new Exception("Employee with those credentials not found."));
            }
            else if (employeeExists.PositionID == 3 || employeeExists.PositionID == 5)
            {
                employeeInfo = _context.Employees
                                    .Where(x => x.EmployeeID == employeeExists.EmployeeID)
                                    .Select(x => new EmployeeInfo()
                                    {
                                        EmployeeID = x.EmployeeID,
                                        FirstName = x.FirstName,
                                        LastName = x.LastName,
                                        IsManager = true
                                    })
                                    .SingleOrDefault();
            }
            else
            {
                errorList.Add(new Exception("Employee is not authorized to access sales content."));
            }

            if (errorList.Count > 0)
            {
                //  throw the list of business processing error(s)
                throw new AggregateException("Error with employee login. Check concerns", errorList);
            }
            else
            {
                return employeeInfo;
            }
        }

        public EmployeeInfo GetEmployeeInfoPurchasing(int loginID, string phone)
        {
            bool employeeExist = _context.Employees.Where(x => x.EmployeeID == loginID && x.ContactPhone == phone).Any();
            EmployeeInfo employee = null;
            if(employeeExist==false)
            {
                throw new Exception("Employee does not exist");
            }    
            if(loginID == 3 || loginID == 13)
            {
                employee = _context.Employees.Where(x => x.EmployeeID == loginID).Select(x=> new EmployeeInfo
                { 
                   EmployeeID = loginID,
                   FirstName = x.FirstName,
                   LastName = x.LastName,
                   IsManager = true
                }).FirstOrDefault();
            }
            else
            {
                throw new Exception("Only department head can view this page");
            }
            return employee;
        }
        #endregion
    }
}
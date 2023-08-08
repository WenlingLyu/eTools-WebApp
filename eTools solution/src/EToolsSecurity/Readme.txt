
Here is the library for security/employee.  The person who is responsible for maintaining the application will add this library to your solution. 

You will need to add a  using clause to your project
using SecurityDependencies;

You will need to add the security to the IServiceCollection
builder.Services.SecuritySystemBackendDependencies(options =>
    options.UseSqlServer(xxxxxxxx));


Each member will then add a dependency to your project.  You will need to use the service by adding it to the constructor of your page.


public IndexModel([existing services], SecurityService securityServices)

{

}


To use it, you will need to create a property on your page

public EmployeeInfo Employee {get; set;}

Then you will need to update it as you are using it (True if the employee is manager)

Employee = _securityService.GetEmployeeInfo(true);

Then you can reference the employee as needed.

James


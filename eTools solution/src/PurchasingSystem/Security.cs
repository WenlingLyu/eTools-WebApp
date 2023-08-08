namespace PurchasingSystem
{
    public static class Security
    {
        //  set the value to true if you are log in as a manager.
        private static bool isManager = true;

        //  Store Manger is 3, Associate are 1
        public static int EmployeeId()
        {
            return isManager ? 3 : 1;
        }

    }
}

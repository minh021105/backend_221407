namespace backend_221407
{
    public class Global
    {
        public static String getConnectString()
        {
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("Northwind_conn");
            return conStr;


        }

        //Ứng dụng Windows Form thì lấy kết nối thế này
        /*
        public static String getConnectionString()
        {
            String getstr =
           ConfigurationManager.ConnectionStrings["Northwind_conn"].ConnectionString;
            return getstr;
        }
        */

        public static int ToInt(object obj)
        {
            int i = 0;
            int.TryParse(obj.ToString(), out i);
            return i;
        }

        public static decimal ToDecimal(object obj)
        {
            decimal i = 0;
            decimal.TryParse(obj.ToString(), out i);
            return i;
        }

    }
}


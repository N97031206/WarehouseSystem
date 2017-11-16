using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc4;
using System.Web.Configuration;
using WarehouseSystem.Models.Interface;
using WarehouseSystem.Models.Repository;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service;
using WarehouseSystem.Models.DbContextFactory;

namespace WarehouseSystem
{
    public static class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            return container;
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();    
            RegisterTypes(container);

            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            string ConnectionString = WebConfigurationManager.ConnectionStrings["WarehouseServerEntities"].ConnectionString;

            container.RegisterType<IDbContextFactory, DbContextFactory>(
                new HierarchicalLifetimeManager(),
                new InjectionConstructor(ConnectionString));

            //Repository
            container.RegisterType<IRepository<CST_USER_PROFILE>, GenericRepository<CST_USER_PROFILE>>();
            container.RegisterType<IRepository<CST_PALLET_TYPE>, GenericRepository<CST_PALLET_TYPE>>();
            container.RegisterType<IRepository<CST_PALLET>, GenericRepository<CST_PALLET>>();
            container.RegisterType<IRepository<CST_RFID>, GenericRepository<CST_RFID>>();
            container.RegisterType<IRepository<CST_GATE_READER>, GenericRepository<CST_GATE_READER>>();
            container.RegisterType<IRepository<CST_MENU_MAIN>, GenericRepository<CST_MENU_MAIN>>();
            container.RegisterType<IRepository<CST_MENU_SUB>, GenericRepository<CST_MENU_SUB>>();
            container.RegisterType<IRepository<CST_MENU_MINOR>, GenericRepository<CST_MENU_MINOR>>();
            container.RegisterType<IRepository<CST_USER_GRP>, GenericRepository<CST_USER_GRP>>();
            container.RegisterType<IRepository<CST_MENU_GRP>, GenericRepository<CST_MENU_GRP>>();
            container.RegisterType<IRepository<CST_FIELD_MAP>, GenericRepository<CST_FIELD_MAP>>();
            container.RegisterType<IRepository<CST_BEACON_LOCATION>, GenericRepository<CST_BEACON_LOCATION>>();
            container.RegisterType<IRepository<CST_STORAGE_PARKINGBLOCK>, GenericRepository<CST_STORAGE_PARKINGBLOCK>>();
            container.RegisterType<IRepository<CST_WAREHOUSE>, GenericRepository<CST_WAREHOUSE>>();
            container.RegisterType<IRepository<CST_ALARM>, GenericRepository<CST_ALARM>>();
            container.RegisterType<IRepository<CST_MAIL_GRP>, GenericRepository<CST_MAIL_GRP>>();
            container.RegisterType<IRepository<CST_MAIL_GRP_DTL>, GenericRepository<CST_MAIL_GRP_DTL>>();
            container.RegisterType<IRepository<CST_RFID_ERROR_LOG>, GenericRepository<CST_RFID_ERROR_LOG>>();
            container.RegisterType<IRepository<CST_INV>, GenericRepository<CST_INV>>();
            container.RegisterType<IRepository<CST_INV_DTL>, GenericRepository<CST_INV_DTL>>();
            container.RegisterType<IRepository<CST_SER_INV>, GenericRepository<CST_SER_INV>>();
            container.RegisterType<IRepository<CST_SER_INV_DTL>, GenericRepository<CST_SER_INV_DTL>>();
            container.RegisterType<IRepository<CST_STOCK_OUT>, GenericRepository<CST_STOCK_OUT>>();
            container.RegisterType<IRepository<CST_STOCK_OUT_LOG>, GenericRepository<CST_STOCK_OUT_LOG>>();
            container.RegisterType<IRepository<CST_WAREHOUSE_LOG>, GenericRepository<CST_WAREHOUSE_LOG>>();

            //Service
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IPalletTypeService, PalletTypeService>();
            container.RegisterType<IPalletService, PalletService>();
            container.RegisterType<IRFIDService, RFIDService>();
            container.RegisterType<IGateReaderService, GateReaderService>();
            container.RegisterType<IMainMenuService, MainMenuService>();
            container.RegisterType<ISubMenuService, SubMenuService>();
            container.RegisterType<IMinorMenuService, MinorMenuService>();
            container.RegisterType<IUserGroupService, UserGroupService>();
            container.RegisterType<IMenuGroupService, MenuGroupService>();
            container.RegisterType<IFieldService, FieldService>();
            container.RegisterType<IStorageParkingBlockService, StorageParkingBlockService>();
            container.RegisterType<IBeaconService, BeaconService>();
            container.RegisterType<IWarehouseService, WarehouseService>();
            container.RegisterType<IAlarmService, AlarmService>();
            container.RegisterType<IMailGroupService, MailGroupService>();
            container.RegisterType<IMailDetailService, MailDetailService>();
            container.RegisterType<IRFIDErrorMessage, RFIDErrorMessage>();
            container.RegisterType<IInventoryService, InventoryService>();
            container.RegisterType<IInventoryDetailService, InventoryDetailService>();
            container.RegisterType<IFindGoodsService, FindGoodsService>();
            container.RegisterType<IFindGoodsDetailService, FindGoodsDetailService>();
            container.RegisterType<IRecord, Record>();
            container.RegisterType<IWarehouseLogService, WarehouseLogService>();
            container.RegisterType<IStockOutService, StockOutService>();
            container.RegisterType<IStockOutLogService, StockOutLogService>();
        }
    }
}
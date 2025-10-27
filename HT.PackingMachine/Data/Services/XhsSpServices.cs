using Dapper;
using HT.PackingMachine.Data.Entities;
using HT.PackingMachine.Data.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HT.PackingMachine.Data.Services
{
    public class XhsSpServices
    {
        public IDbConnection Connection { get { return new SqlConnection(_connectionString); } }
        const string SchemaName = "xhs";
        readonly string _connectionString;

        private readonly ILogger<XhsSpServices> _logger;
        public XhsSpServices(IConfiguration configuration, ILogger<XhsSpServices> logger)
        {
            _connectionString = configuration["ConnectionStrings:XhsDbContext"] ?? "";
            _logger = logger;
        }
        #region BatchNumList
        public void P_BatchNumList_Create(BatchNum_List BatchNumList)
        {
            using (var connection = Connection)
            {
                if (connection.State != ConnectionState.Open) { connection.Open(); }

                string sql = SchemaName + ".P_BatchNumList_Create";
                try
                {
                    var pars = new DynamicParameters();
                    pars.AddDynamicParams(new
                    {
                        BatchNum = BatchNumList.BatchNum,
                        Category_Id = BatchNumList.Category_Id,
                        Qty = BatchNumList.Qty,
                        CreatedBy = BatchNumList.CreatedBy,
                        CreatedDate = BatchNumList.CreatedDate,
                        ModifiedBy = BatchNumList.ModifiedBy,
                        ModifiedDate = BatchNumList.ModifiedDate,
                        Status = BatchNumList.Status
                    });
                    connection.Query(sql: sql, param: pars,
                    commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
        public void P_BatchNumList_Update(BatchNum_List BatchNumList)
        {
            using (var connection = Connection)
            {
                if (connection.State != ConnectionState.Open) { connection.Open(); }

                string sql = SchemaName + ".P_BatchNumList_Update";
                try
                {
                    var pars = new DynamicParameters();
                    pars.AddDynamicParams(new
                    {
                        ID = BatchNumList.Id,
                        Category_Id = BatchNumList.Category_Id,
                        Qty = BatchNumList.Qty,
                        ModifiedBy = BatchNumList.ModifiedBy,
                        ModifiedDate = BatchNumList.ModifiedDate,
                    });
                    connection.Query(sql: sql, param: pars,
                    commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
        public void P_BatchNumList_Cancel(BatchNum_List BatchNumList)
        {
            using (var connection = Connection)
            {
                if (connection.State != ConnectionState.Open) { connection.Open(); }

                string sql = SchemaName + ".P_BatchNumList_Cancel";
                try
                {
                    var pars = new DynamicParameters();
                    pars.AddDynamicParams(new
                    {
                        ID = BatchNumList.Id,
                        Status = BatchNumList.Status,
                        ModifiedBy = BatchNumList.ModifiedBy,
                        ModifiedDate = BatchNumList.ModifiedDate,
                    });
                    connection.Query(sql: sql, param: pars,
                    commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
        #endregion
        #region BatchNumQuality
        public void P_BatchNumQuality_Update(SoLoChatLuongModel BatchNumList)
        {
            using (var connection = Connection)
            {
                if (connection.State != ConnectionState.Open) { connection.Open(); }

                string sql = SchemaName + ".P_BatchNumQuality_Update";
                try
                {
                    var pars = new DynamicParameters();
                    pars.AddDynamicParams(new
                    {
                        ID = BatchNumList.Id,
                        Category_Id = BatchNumList.CategoryId,
                        Qty = BatchNumList.Qty,
                        QualityNum = BatchNumList.QualityNum,
                        QualityDate = BatchNumList.QualityDate,
                        ModifiedBy = BatchNumList.ModifiedBy,
                        ModifiedDate = BatchNumList.ModifiedDate,
                        BeginDate = BatchNumList.BeginDate,
                        EndDate = BatchNumList.EndDate
                    });
                    connection.Query(sql: sql, param: pars,
                    commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
        public void P_BatchNumQuality_Create(SoLoChatLuongModel BatchNumList)
        {
            using (var connection = Connection)
            {
                if (connection.State != ConnectionState.Open) { connection.Open(); }

                string sql = SchemaName + ".P_BatchNumQuality_Create";
                try
                {
                    var pars = new DynamicParameters();
                    pars.AddDynamicParams(new
                    {
                        BatchNum = BatchNumList.BatchNum,
                        Category_Id = BatchNumList.CategoryId,
                        Qty = BatchNumList.Qty,
                        BeginDate = BatchNumList.BeginDate,
                        EndDate = BatchNumList.EndDate,
                        QualityDate = BatchNumList.QualityDate,
                        QualityNum = BatchNumList.QualityNum,
                        CreatedBy = BatchNumList.CreatedBy,
                        CreatedDate = BatchNumList.CreatedDate,
                        ModifiedBy = BatchNumList.ModifiedBy,
                        ModifiedDate = BatchNumList.ModifiedDate,
                        Status = BatchNumList.Status
                    });
                    connection.Query(sql: sql, param: pars,
                    commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
        #endregion
        #region BatchNumRegister
        public void P_BatchNumRegister_Create(BatchNum_Register BatchNumRegister)
        {
            using (var connection = Connection)
            {
                if (connection.State != ConnectionState.Open) { connection.Open(); }

                string sql = SchemaName + ".P_BatchNumRegisters_Create";
                try
                {
                    var pars = new DynamicParameters();
                    pars.AddDynamicParams(new
                    {
                        BatchNum_List_Id = BatchNumRegister.BatchNum_List_Id,
                        PackingMachineId = BatchNumRegister.PackingMachine_Id,
                        Qty = BatchNumRegister.Qty,
                        CreatedBy = BatchNumRegister.CreatedBy,
                        CreatedDate = BatchNumRegister.CreatedDate,
                        ModifiedBy = BatchNumRegister.ModifiedBy,
                        ModifiedDate = BatchNumRegister.ModifiedDate,
                        Status = BatchNumRegister.Status
                    });
                    connection.Query(sql: sql, param: pars,
                    commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
        public void P_BatchNumRegister_UpdateID(BatchNum_Register BatchNumRegister)
        {
            using (var connection = Connection)
            {
                if (connection.State != ConnectionState.Open) { connection.Open(); }

                string sql = SchemaName + ".P_BatchNumRegisters_UpdateID";
                try
                {
                    var pars = new DynamicParameters();
                    pars.AddDynamicParams(new
                    {
                        ID = BatchNumRegister.Id,
                        ModifiedBy = BatchNumRegister.ModifiedBy,
                        ModifiedDate = BatchNumRegister.ModifiedDate,
                        Status = BatchNumRegister.Status
                    });
                    connection.Query(sql: sql, param: pars,
                    commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
        public void P_BatchNumRegister_Update(BatchNum_Register BatchNumRegister)
        {
            using (var connection = Connection)
            {
                if (connection.State != ConnectionState.Open) { connection.Open(); }

                string sql = SchemaName + ".P_BatchNumRegisters_Update";
                try
                {
                    var pars = new DynamicParameters();
                    pars.AddDynamicParams(new
                    {
                        ID = BatchNumRegister.Id,
                        BatchNum_List_Id = BatchNumRegister.BatchNum_List_Id,
                        PackingMachineId = BatchNumRegister.PackingMachine_Id,
                        Qty = BatchNumRegister.Qty,
                        ModifiedBy = BatchNumRegister.ModifiedBy,
                        ModifiedDate = BatchNumRegister.ModifiedDate,
                    });
                    connection.Query(sql: sql, param: pars,
                    commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
        public void P_BatchNumRegister_Cancel(BatchNum_Register BatchNumRegister)
        {
            using (var connection = Connection)
            {
                if (connection.State != ConnectionState.Open) { connection.Open(); }

                string sql = SchemaName + ".P_BatchNumRegisters_Cancel";
                try
                {
                    var pars = new DynamicParameters();
                    pars.AddDynamicParams(new
                    {
                        ID = BatchNumRegister.Id,
                        Status = BatchNumRegister.Status,
                        ModifiedBy = BatchNumRegister.ModifiedBy,
                        ModifiedDate = BatchNumRegister.ModifiedDate,
                    });
                    connection.Query(sql: sql, param: pars,
                    commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
        #endregion
        #region PackingMachineCategory
        public void P_PackingMachineCategory_Create(PackingMachineCategoryHistory packingMachineCategory)
        {
            using (var connection = Connection)
            {
                if (connection.State != ConnectionState.Open) { connection.Open(); }

                string sql = SchemaName + ".P_PackingMachineCategory_Create";
                try
                {
                    var pars = new DynamicParameters();
                    pars.AddDynamicParams(new
                    {
                        PackingMachineId = packingMachineCategory.PackingMachine_Id,
                        CategoryId = packingMachineCategory.Category_Id,
                        BeginDate = packingMachineCategory.BeginDate,
                        CreatedBy = packingMachineCategory.CreatedBy,
                        CreatedDate = packingMachineCategory.CreatedDate,
                        ModifiedDate = packingMachineCategory.ModifiedDate,
                        Status = packingMachineCategory.Status
                    });
                    connection.Query(sql: sql, param: pars,
                    commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
        public void P_PackingMachineCategory_Cancel(PackingMachineCategoryHistory packingMachineCategory)
        {
            using (var connection = Connection)
            {
                if (connection.State != ConnectionState.Open) { connection.Open(); }

                string sql = SchemaName + ".P_PackingMachineCategory_Cancel";
                try
                {
                    var pars = new DynamicParameters();
                    pars.AddDynamicParams(new
                    {
                        ID = packingMachineCategory.Id,
                        EndDate = packingMachineCategory.EndDate,
                        ModifiedBy = packingMachineCategory.ModifiedBy,
                        ModifiedDate = packingMachineCategory.ModifiedDate,
                    });
                    connection.Query(sql: sql, param: pars,
                    commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
        public void P_PackingMachineCategory_Delete(PackingMachineCategoryHistory packingMachineCategory)
        {
            using (var connection = Connection)
            {
                if (connection.State != ConnectionState.Open) { connection.Open(); }

                string sql = SchemaName + ".P_PackingMachineCategory_Delete";
                try
                {
                    var pars = new DynamicParameters();
                    pars.AddDynamicParams(new
                    {
                        ID = packingMachineCategory.Id,
                        EndDate = packingMachineCategory.EndDate,
                        ModifiedBy = packingMachineCategory.ModifiedBy,
                        ModifiedDate = packingMachineCategory.ModifiedDate,
                        Status = packingMachineCategory.Status
                    });
                    connection.Query(sql: sql, param: pars,
                    commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
        public void P_PackingMachineCategory_Update(PackingMachineCategoryHistory packingMachineCategory)
        {
            using (var connection = Connection)
            {
                if (connection.State != ConnectionState.Open) { connection.Open(); }

                string sql = SchemaName + ".P_PackingMachineCategory_Update";
                try
                {
                    var pars = new DynamicParameters();
                    pars.AddDynamicParams(new
                    {
                        ID = packingMachineCategory.Id,
                        ModifiedBy = packingMachineCategory.ModifiedBy,
                        ModifiedDate = packingMachineCategory.ModifiedDate,

                    });
                    connection.Query(sql: sql, param: pars,
                    commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
        #endregion

        #region CheckOut
        public int CheckoutWaitingDelivery(Guid GID, int CheckoutId, string VehicleCode, int LocationId)
        {
            string Error_Msg = null;
            using (var connection = Connection)
            {
                connection.Open();
                string sql = "xhs.p_Checkout_WaitingDelivery_V1";

                try
                {
                    var pars = new DynamicParameters();
                    pars.AddDynamicParams(new
                    {
                        GID = GID,
                        CheckoutId = CheckoutId,
                        VehicleCode = VehicleCode,
                        LocationId = LocationId
                    });

                    var ret = connection.Query<int>(sql: sql, param: pars,

                     commandType: CommandType.StoredProcedure);

                    return 1;
                }
                catch (Exception ex)
                {
                    Error_Msg = ex.Message;
                }

            }
            return 0;
        }
        #endregion
        #region VehicleCardNumber
        public List<SalesOrderViewModel> GetSalesOrderByDeliveryCode(string maGiaoHang)
        {
            string Error_Msg = null;
            IEnumerable<SalesOrderViewModel>? salesOrders = new List<SalesOrderViewModel>();
            using (var connection = Connection)
            {
                connection.Open();
                string sql = "xhs.P_Get_Sales_Orders";

                try
                {
                    var pars = new DynamicParameters();
                    pars.AddDynamicParams(new
                    {
                        GID = (Guid?)null,
                        VehicleCode = (string?)null,
                        From_ORDER_DATE = (string?)null,
                        To_ORDER_DATE = (string?)null,
                        ORDER_ID = (string?)null,
                        DELIVERY_CODE = maGiaoHang,
                        STATUS = (string?)null,
                        ItemLineName = (string?)null,
                        ItemTypeName = (string?)null,
                        TRIP_NUMBER = (string?)null,
                        CUSTOMER_ID = (string?)null
                    });


                    salesOrders = connection.Query<SalesOrderViewModel>(sql: sql, param: pars,

                    commandType: CommandType.StoredProcedure);

                    return salesOrders.ToList();
                }
                catch (Exception ex)
                {
                    Error_Msg = ex.Message;
                }

            }
            return salesOrders.ToList();
        }
        public string f_Vehicle_Code(string vehicleCode)
        {
            using (var connection = Connection)
            {
                connection.Open();
                string sql = $"SELECT xhs.f_Vehicle_Code('{vehicleCode}')";
                try
                {
                    var ret = connection.Query<string>(sql: sql,
                     commandType: CommandType.Text);

                    return ret.First();
                }
                catch (Exception)
                {
                    //_logger.LogError(ex, $"Lỗi {System.Reflection.MethodInfo.GetCurrentMethod()}");
                }
                finally
                {
                    if (connection.State == ConnectionState.Open) connection.Close();
                }
            }
            return String.Empty;
        }
     
        #endregion
    }
}

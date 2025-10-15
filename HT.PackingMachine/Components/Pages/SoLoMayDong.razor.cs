using HT.PackingMachine.Data;
using HT.PackingMachine.Data.Entities;
using HT.PackingMachine.Data.Enums;
using HT.PackingMachine.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using System.Security.Claims;

namespace HT.PackingMachine.Components.Pages
{
    public partial class SoLoMayDong
    {
        [Inject] XhsContext? XhsContext { get; set; }
        [Inject] XhsSpServices? XhsSpServices { get; set; }
        [Inject] ISnackbar? Snackbar { get; set; }
        [Inject] NavigationManager? NavigationManager { get; set; }
        private BatchNum_Register Input { get; set; } = new();

        private List<BatchNum_Register> BatchNumRegisters { get; set; } = new();
        private List<Data.Entities.PackingMachine> PackingMachines { get; set; } = new();

        private List<BatchNum_List> BatchNumLists { get; set; } = new();

        private bool ShowProgress;
        private string searchString = "";

        public string ShowEdit = "d-none";
        public string ShowDelete = "d-none";
        public string ShowCreate = "d-flex";


        [CascadingParameter] private Task<AuthenticationState>? AuthenticationStateTask { get; set; }
        public int UserId = 0;
        protected override async Task OnInitializedAsync()
        {


            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
              
                await HandleToken();
                await GetBatchNumListAsync();
                await GetPackingMachineAsync();
                await GetBatchNumRegisterAsync();
            
                StateHasChanged();
            }
        }
        private async Task HandleToken()
        {
            // kiểm tra refreshToken đang có, tồn tại hay ko?
            var refreshToken = await JSRuntime.InvokeAsync<string>("getCookie", "refreshToken");
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var token = await AuthService.CheckTokenAndRefresh(NavigationManager.BaseUri);
                if (string.IsNullOrEmpty(token))
                {
                    // là null quay lại đăng nhập
                    NavigationManager.NavigateTo("/Logout");
                }
            }
            else
            {
                // nếu ko tồn tại quay lại đăng nhập
                NavigationManager.NavigateTo("/Logout");
            }
        }
        private async Task Reload(string message, Severity severity)
        {
            await Task.Delay(300);
            ShowCreate = "d-flex";
            ShowEdit = "d-none";
            ShowDelete = "d-none";
            Snackbar!.Add(message, severity);

            await GetBatchNumListAsync();
            await GetPackingMachineAsync();
            await GetBatchNumRegisterAsync();
            Input = new();
            await InvokeAsync(StateHasChanged);
        }
        private async void OnValidSubmit(EditContext context)
        {
            await Create();
            StateHasChanged();
        }
        private async Task Create()
        {
            ShowProgress = true;
            await Task.Delay(300);
            //if (UserId == 0) return;
            try
            {
                //Kiểm tra số lô có tồn tại hay ko
                var checkBatchNumList = await XhsContext!.BatchNum_Lists
                           .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable && a.Id == Input.BatchNum_List_Id).ToListAsync();

                if (checkBatchNumList.Any())
                {

                    var qtyOfBatchNumList = checkBatchNumList.First().Qty;
                    //Nêu số lượng yêu cầu nhỏ hơn hoặc bằng số lượng của số lô thì được phép thêm mới
                    if (Input.Qty <= qtyOfBatchNumList)
                    {
                        BatchNum_Register batchNum_Register = new BatchNum_Register
                        {
                            BatchNum_List_Id = Input.BatchNum_List_Id,
                            PackingMachine_Id = Input.PackingMachine_Id,
                            Qty = Input.Qty,
                            CreatedBy = UserId,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = UserId,
                            ModifiedDate = DateTime.Now,
                            Status = (byte)HTStatusCodeEnum.Stable
                        };

                        try
                        {
                            XhsSpServices!.P_BatchNumRegister_Create(batchNum_Register);
                        }
                        catch (Exception ex)
                        {
                            Snackbar?.Add(ex.Message, Severity.Error);

                        }

                        //sau khi thêm mới
                        //Lấy bản ghi mới nhất vừa thêm mới
                        var firstBatchNumRegister = await XhsContext.BatchNum_Registers
                      .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable)
                      .Where(s => s.PackingMachine_Id == Input.PackingMachine_Id)
                      .OrderByDescending(s => s.Id).ToListAsync();


                        if (firstBatchNumRegister.Any())
                        {
                            var first = firstBatchNumRegister.FirstOrDefault();

                            //cập nhật lại tất cả các bản ghi có Máy đóng cùng loại trừ bản ghi mới nhất
                            var updateBatchNumRegisterUpdate = await XhsContext.BatchNum_Registers
                             .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable)
                             .Where(s => s.PackingMachine_Id == Input.PackingMachine_Id)
                             .Where(s => s.Id != first!.Id)
                             .ToListAsync();

                            if (updateBatchNumRegisterUpdate.Any())
                            {
                                foreach (var item in updateBatchNumRegisterUpdate)
                                {
                                    BatchNum_Register batchNumRegister = new BatchNum_Register
                                    {
                                        Id = item.Id,
                                        ModifiedBy = UserId,
                                        ModifiedDate = DateTime.Now,
                                        Status = (byte)HTStatusCodeEnum.Complete
                                    };
                                    try
                                    {
                                        XhsSpServices.P_BatchNumRegister_UpdateID(batchNumRegister);
                                    }
                                    catch (Exception ex)
                                    {
                                        Snackbar?.Add(ex.Message, Severity.Error);

                                    }

                                }
                            }
                        }

                        await Reload("Thêm số lô cho máy đóng thành công!", Severity.Success);
                    }
                    else
                    {
                        await Reload("Số lượng vượt quá sô lượng của số lô!", Severity.Warning);
                    }



                }
            }
            catch (Exception ex)
            {
                await Reload(ex.Message, Severity.Error);

            }
            ShowProgress = false;
        }
        private async Task Delete()
        {
            ShowProgress = true;
            await Task.Delay(300);
            //if (UserId == 0) return;
            try
            {
                //Kiểm tra có tồn tại hay ko
                var updateBatchNumRegisterCancel = await XhsContext!.BatchNum_Registers
                        .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable && a.Id == Input.Id).ToListAsync();

                //nếu có được xóa
                if (updateBatchNumRegisterCancel.Any())
                {
                    BatchNum_Register batchNumRegister = new BatchNum_Register
                    {
                        Id = Input.Id,
                        ModifiedBy = UserId,
                        ModifiedDate = DateTime.Now,
                        Status = (byte)HTStatusCodeEnum.Delete
                    };
                    try
                    {
                        XhsSpServices!.P_BatchNumRegister_Cancel(batchNumRegister);
                    }
                    catch (Exception ex)
                    {
                        Snackbar?.Add(ex.Message, Severity.Error);

                    }
                    await Reload("Xóa số lô cho máy đóng thành công!", Severity.Success);
                }
            }
            catch (Exception ex)
            {
                Snackbar?.Add(ex.Message, Severity.Error);

            }
            ShowProgress = false;
            StateHasChanged();
        }
        private async Task Update()
        {
            ShowProgress = true;
            await Task.Delay(300);
            //if (UserId == 0) return;
            try
            {
                //Kiểm tra có tồn tại hay ko
                var checkBatchNumList = await XhsContext!.BatchNum_Lists
                          .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable && a.Id == Input.BatchNum_List_Id).ToListAsync();

                //nếu có được cập nhật
                if (checkBatchNumList.Any())
                {
                    var qtyOfBatchNumList = checkBatchNumList.First().Qty;

                    //nếu số lượng cập nhật thỏa mãn <= số lượng của số lô
                    if (Input.Qty <= qtyOfBatchNumList)
                    {
                        BatchNum_Register batchNumRegister = new BatchNum_Register
                        {
                            Id = Input.Id,
                            BatchNum_List_Id = Input.BatchNum_List_Id,
                            PackingMachine_Id = Input.PackingMachine_Id,
                            Qty = Input.Qty,
                            ModifiedBy = UserId,
                            ModifiedDate = DateTime.Now
                        };
                        try
                        {
                            XhsSpServices!.P_BatchNumRegister_Update(batchNumRegister);
                        }
                        catch (Exception ex)
                        {
                            Snackbar?.Add(ex.Message, Severity.Error);

                        }

                        //sau khi cập nhật
                        //lấy tất cả bản ghi có máy đóng cùng loại trừ bản ghi vùa cập nhật
                        //cập nhật trạng thái 
                        var checkUpdate = await XhsContext.BatchNum_Registers
                       .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable)
                       .Where(s => s.PackingMachine_Id == Input.PackingMachine_Id)
                       .Where(x => x.Id != Input.Id).ToListAsync();

                        if (checkUpdate.Any())
                        {
                            foreach (var item in checkUpdate)
                            {
                                BatchNum_Register batchNumRegisterUpdate = new BatchNum_Register
                                {
                                    Id = item.Id,
                                    ModifiedBy = UserId,
                                    ModifiedDate = DateTime.Now,
                                    Status = (byte)HTStatusCodeEnum.Complete
                                };
                                try
                                {
                                    XhsSpServices.P_BatchNumRegister_UpdateID(batchNumRegisterUpdate);
                                }
                                catch (Exception ex)
                                {
                                    Snackbar?.Add(ex.Message, Severity.Error);

                                }
                            }

                        }

                        await Reload("Cập nhật số lô cho máy đóng thành công!", Severity.Success);

                    }
                    else
                    {
                        Snackbar!.Add("Số lượng vượt quá sô lượng của số lô!", Severity.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar?.Add(ex.Message, Severity.Error);

            }
            ShowProgress = false;
            StateHasChanged();
        }
        private async Task Back()
        {
            await Reload("", Severity.Normal);
        }
        private async Task SelectBatchNumRegister(int id)
        {
            Input = new();
            ShowEdit = "d-flex";
            ShowDelete = "d-flex";
            ShowCreate = "d-none";
            var data = await XhsContext!.BatchNum_Registers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (data != null)
            {
                Input = new()
                {
                    Id = data.Id,
                    BatchNum_List_Id = data.BatchNum_List_Id,
                    PackingMachine_Id = data.PackingMachine_Id,
                    Qty = data.Qty
                };
            }
        }
        private bool FilterFunc(BatchNum_Register element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.BatchNum_List!.BatchNum.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
        private async Task<List<BatchNum_Register>> GetBatchNumRegisterAsync()
        {
            BatchNumRegisters = new();
            var data = await XhsContext!.BatchNum_Registers.Include(p => p.BatchNum_List).Include(p => p.PackingMachine)
                .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable)
                .Where(p => p.PackingMachine.PackingType == (int)HTPackingTypeEnum.BAO)
                .OrderByDescending(c => c.Id).AsNoTracking().ToListAsync();

            foreach (var item in data)
            {
                BatchNum_Register batchNum = new()
                {
                    Id = item.Id,
                    BatchNum_List = item.BatchNum_List,
                    PackingMachine = item.PackingMachine,
                    BatchNum_List_Id = item.BatchNum_List_Id,
                    PackingMachine_Id = item.PackingMachine_Id,
                    Qty = item.Qty,
                };

                BatchNumRegisters.Add(batchNum);
            }

            return BatchNumRegisters;
        }
        private async Task<List<BatchNum_List>> GetBatchNumListAsync()
        {
            BatchNumLists = new();
            BatchNumLists = await XhsContext!.BatchNum_Lists.Include(x => x.Category).Where(x => x.Category.PackingType == (int)HTPackingTypeEnum.BAO)
                .Where(x => x.Status == (byte)HTStatusCodeEnum.Stable).AsNoTracking().ToListAsync();
            return BatchNumLists;
        }

        private async Task<List<Data.Entities.PackingMachine>> GetPackingMachineAsync()
        {
            PackingMachines = new();
            PackingMachines = await XhsContext!.PackingMachines
                .Where(p => p.PackingType == (int)HTPackingTypeEnum.BAO)
             .Where(x => x.Status == (byte)HTStatusCodeEnum.Stable)
             .AsNoTracking().ToListAsync();

            return PackingMachines;
        }

        private int _batchNumListId
        {
            get => Input.BatchNum_List_Id;
            set
            {
                if (value > 0)
                {
                    Input.BatchNum_List_Id = value;
                    PackingMachines = new();
                    var checkBatchNumList = XhsContext!.BatchNum_Lists.Where(s => s.Id == Input.BatchNum_List_Id).ToList();
                    if (checkBatchNumList.Any())
                    {
                        //Thay đổi máy đóng dựa theo Số lô
                        int categoryId = checkBatchNumList.First().Category_Id;

                        var pm = (from c in XhsContext.PackingMachines.Where(p => p.PackingType == (int)HTPackingTypeEnum.BAO && p.Status == (byte)HTStatusCodeEnum.Stable)
                                  join d in XhsContext.PackingMachineCategoryHistories.Where(p => p.Status == (byte)HTStatusCodeEnum.Stable) on c.Id equals d.PackingMachine_Id
                                  where d.Category_Id == categoryId
                                  select new { c.Id, c.Name }).Distinct();

                        foreach (var item in pm)
                        {
                            Data.Entities.PackingMachine packingMachine = new()
                            {
                                Id = item.Id,
                                Name = item.Name
                            };

                            PackingMachines.Add(packingMachine);
                        }
                    }
                }
            }
        }

        private void RowClickEvent(TableRowClickEventArgs<BatchNum_Register> tableRowClickEventArgs)
        {
            Input = new();
        }
    }
}

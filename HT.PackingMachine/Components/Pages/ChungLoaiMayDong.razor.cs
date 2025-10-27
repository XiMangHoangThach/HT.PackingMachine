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
using System.Reflection.Emit;
using System.Security.Claims;

namespace HT.PackingMachine.Components.Pages
{
    public partial class ChungLoaiMayDong
    {
        [Inject] XhsContext? XhsContext { get; set; }
        [Inject] XhsSpServices? XhsSpServices { get; set; }
        [Inject] ISnackbar? Snackbar { get; set; }
        [Inject] NavigationManager? NavigationManager { get; set; }
        private PackingMachineCategoryHistory Input { get; set; } = new();

        private List<PackingMachineCategoryHistory> packingMachineCategoryHistories { get; set; } = new();
        private List<Data.Entities.PackingMachine> packingMachines { get; set; } = new();

        private List<Category> Categories { get; set; } = new();

        private bool ShowProgress;
        private string searchString = "";

        public string ShowEdit = "d-none";
        public string ShowDelete = "d-none";
        public string ShowCreate = "d-flex";
        public bool DisabledSoLo = false;


        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        public int UserId = 0;
        public string? Gid = "";
        protected override async Task OnInitializedAsync()
        {


            await base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await HandleToken();

                await GetCategoryAsync();
                await GetPackingMachineAsync();
                await GetPackingMachineCategoryAsync();
         
                StateHasChanged();
            }
        }
        private async Task HandleToken()
        {
            var token = await AuthService.CheckTokenAndRefresh(null);

            if (authenticationState is not null)
            {
                var authState = await authenticationState;
                var user = authState?.User;

                if (user?.Identity is not null && user.Identity.IsAuthenticated && !string.IsNullOrEmpty(token))
                {
                    UserId = int.Parse(user.Claims.First().Value);
                    Gid = user.FindFirst(c => c.Type == "Gid")?.Value;
                }
                else
                {
                    NavigationManager.NavigateTo("Login");
                }
            }
            else
            {
                NavigationManager.NavigateTo("Login");
            }
        }
        private async Task Reload(string message, Severity severity)
        {
            await Task.Delay(300);
            ShowCreate = "d-flex";
            ShowEdit = "d-none";
            ShowDelete = "d-none";
            Snackbar!.Add(message, severity);
            DisabledSoLo = false;
            await GetCategoryAsync();
            await GetPackingMachineAsync();
            await GetPackingMachineCategoryAsync();

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

            // if (UserId == 0) return;

            try
            {
                //Kiểm tra có tồn tại bản ghi tương tự hay ko
                var checkPackingMachineIdAndCategoryId = await XhsContext!.PackingMachineCategoryHistories
                  .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable)
                  .Where(x => x.PackingMachine_Id == Input.PackingMachine_Id)
                  .Where(x => x.Category_Id == Input.Category_Id)
                  .OrderByDescending(a => a.Id).ToListAsync();

                if (checkPackingMachineIdAndCategoryId.Any())
                {
                    Snackbar!.Add("Đang có máy đóng chạy chủng loại tương tự chưa kết thúc!", Severity.Warning);
                }
                else
                {
                    //Nếu không được phép thêm mói
                    //Lây bản ghi mới nhất theo máy đóng
                    var enddateOfprevious = await XhsContext.PackingMachineCategoryHistories
                        .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable)
                        .Where(x => x.PackingMachine_Id == Input.PackingMachine_Id)
                        .OrderByDescending(a => a.Id).ToListAsync();


                    if (enddateOfprevious.Any())
                    {
                        var edO = enddateOfprevious.First();
                        //Cập nhật bản ghi trước đó
                        PackingMachineCategoryHistory packing = new()
                        {
                            Id = edO.Id,
                            EndDate = DateTime.Now,
                            ModifiedBy = UserId,
                            ModifiedDate = DateTime.Now
                        };

                        try
                        {
                            XhsSpServices?.P_PackingMachineCategory_Cancel(packing);
                        }
                        catch (Exception ex)
                        {
                            Snackbar!.Add(ex.Message, Severity.Error);

                        }

                        //Sau đó thêm mới
                        PackingMachineCategoryHistory packingMachine = new()
                        {
                            PackingMachine_Id = Input.PackingMachine_Id,
                            Category_Id = Input.Category_Id,
                            BeginDate = DateTime.Now,
                            CreatedBy = UserId,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            Status = (int)HTStatusCodeEnum.Stable
                        };
                        try
                        {
                            XhsSpServices?.P_PackingMachineCategory_Create(packingMachine);
                        }
                        catch (Exception ex)
                        {
                            Snackbar!.Add(ex.Message, Severity.Error);

                        }


                        await Reload("Thêm mới thành công!", Severity.Success);
                    }
                    else
                    {
                        //Nếu không có bản ghi nào tương tự thì thêm mới
                        PackingMachineCategoryHistory packing = new()
                        {
                            PackingMachine_Id = Input.PackingMachine_Id,
                            Category_Id = Input.Category_Id,
                            BeginDate = DateTime.Now,
                            CreatedBy = UserId,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            Status = (byte)HTStatusCodeEnum.Stable
                        };

                        try
                        {
                            XhsSpServices?.P_PackingMachineCategory_Create(packing);
                        }
                        catch (Exception ex)
                        {
                            Snackbar!.Add(ex.Message, Severity.Error);

                        }
                        await Reload("Thêm mới thành công!", Severity.Success);
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
            // if (UserId == 0) return;
            try
            {
                //Kiểm tra có tồn tại hay ko
                var checkPackingMachineCategoryCancel = await XhsContext!.PackingMachineCategoryHistories
                              .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable && a.Id == Input.Id).ToListAsync();
                if (checkPackingMachineCategoryCancel.Any())
                {
                    var cPC = checkPackingMachineCategoryCancel.First();
                    //Nếu tồn tại kiểm tra enddate thỏa mãn
                    if (cPC.EndDate == null || cPC.EndDate < DateTime.Now)
                    {
                        //Cập nhật trước
                        var cancelList = await XhsContext.PackingMachineCategoryHistories
                           .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable)
                           .Where(x => x.PackingMachine_Id == cPC.PackingMachine_Id)
                           .OrderByDescending(a => a.Id)
                            .ToListAsync();

                        //Lấy bản ghi thứ 2
                        var cancelEnddateOfprevious = cancelList.SkipWhile(x => x.Id != cPC.Id).Skip(1).FirstOrDefault();
                        if (cancelEnddateOfprevious != null)
                        {
                            var updateEnddatePreviousCancel = await XhsContext.PackingMachineCategoryHistories
                           .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable && a.Id == cancelEnddateOfprevious.Id).ToListAsync();

                            if (updateEnddatePreviousCancel.Any())
                            {
                                var uEC = updateEnddatePreviousCancel.First();
                                var dateTimeEnd = "";
                                DateTime validValue;

                                //Cập nhật bản ghi thứ 2
                                PackingMachineCategoryHistory packing = new()
                                {
                                    Id = uEC.Id,
                                    EndDate = DateTime.TryParse(dateTimeEnd, out validValue) ? validValue : (DateTime?)null,
                                    ModifiedDate = DateTime.Now,
                                    ModifiedBy = UserId
                                };

                                try
                                {
                                    XhsSpServices!.P_PackingMachineCategory_Cancel(packing);
                                }
                                catch (Exception ex)
                                {
                                    Snackbar!.Add(ex.Message, Severity.Error);

                                }

                            }
                        }


                        //Xong mới xóa
                        PackingMachineCategoryHistory categoryHistory = new()
                        {
                            Id = Input.Id,
                            EndDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            ModifiedBy = UserId,
                            Status = (byte)HTStatusCodeEnum.Delete
                        };
                        try
                        {
                            XhsSpServices!.P_PackingMachineCategory_Delete(categoryHistory);
                        }
                        catch (Exception ex)
                        {
                            Snackbar!.Add(ex.Message, Severity.Error);

                        }
                    }

                    await Reload("Xóa thành công!", Severity.Success);

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
                var packingMachineCategoryUpdate = await XhsContext!.PackingMachineCategoryHistories
                             .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable && a.Id == Input.Id).ToListAsync();

                if (packingMachineCategoryUpdate.Any())
                {
                    //Cập nhật trước
                    var pMC = packingMachineCategoryUpdate.First();
                    PackingMachineCategoryHistory packing = new()
                    {
                        Id = pMC.Id,
                        ModifiedBy = UserId,
                        ModifiedDate = DateTime.Now,
                    };
                    try
                    {
                        XhsSpServices!.P_PackingMachineCategory_Update(packing);
                    }
                    catch (Exception ex)
                    {
                        Snackbar!.Add(ex.Message, Severity.Error);

                    }

                    //Thêm sau
                    PackingMachineCategoryHistory packingMachine = new()
                    {
                        PackingMachine_Id = pMC.PackingMachine_Id,
                        Category_Id = Input.Category_Id,
                        BeginDate = DateTime.Now,
                        CreatedBy = UserId,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        Status = (byte)HTStatusCodeEnum.Stable
                    };
                    try
                    {
                        XhsSpServices!.P_PackingMachineCategory_Create(packingMachine);
                    }
                    catch (Exception ex)
                    {
                        Snackbar!.Add(ex.Message, Severity.Error);

                    }

                    var enddateOfLast = await XhsContext.PackingMachineCategoryHistories
                   .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable)
                   .Where(x => x.PackingMachine_Id == packingMachine.PackingMachine_Id)
                   .OrderByDescending(a => a.Id).ToListAsync();

                    var list = await XhsContext.PackingMachineCategoryHistories
                    .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable)
                    .Where(x => x.PackingMachine_Id == packingMachine.PackingMachine_Id)
                    .OrderByDescending(a => a.Id)
                    .ToListAsync();

                    //Cập nhật bản ghi thứ 2
                    if (enddateOfLast.Any() && list.Any())
                    {
                        var eO = enddateOfLast.First();
                        var enddateOfprevious = list.SkipWhile(x => x.Id != eO.Id).Skip(1).FirstOrDefault();
                        if (enddateOfprevious != null)
                        {
                            var updateEnddatePrevious = await XhsContext.PackingMachineCategoryHistories
                           .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable && a.Id == enddateOfprevious.Id).ToListAsync();

                            if (updateEnddatePrevious.Any())
                            {
                                var uEP = updateEnddatePrevious.First();
                                PackingMachineCategoryHistory categoryHistory = new()
                                {
                                    Id = uEP.Id,
                                    EndDate = DateTime.Now,
                                    ModifiedDate = DateTime.Now,
                                    ModifiedBy = UserId
                                };

                                try
                                {
                                    XhsSpServices?.P_PackingMachineCategory_Cancel(categoryHistory);
                                }
                                catch (Exception ex)
                                {
                                    Snackbar!.Add(ex.Message, Severity.Error);

                                }
                            }

                        }

                    }



                    await Reload("Cập nhật thành công!", Severity.Success);

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
        private async Task SelectPackingMachineCategory(int id)
        {
            Input = new();
            ShowEdit = "d-flex";
            ShowDelete = "d-flex";
            ShowCreate = "d-none";
            DisabledSoLo = true;
            var data = await XhsContext!.PackingMachineCategoryHistories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (data != null)
            {
                Input = new()
                {
                    Id = data.Id,
                    PackingMachine = data.PackingMachine,
                    Category = data.Category,
                    Category_Id = data.Category_Id,
                    PackingMachine_Id = data.PackingMachine_Id,
                };
            }
        }
        private bool FilterFunc(PackingMachineCategoryHistory element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.PackingMachine!.Name.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
        private async Task<List<PackingMachineCategoryHistory>> GetPackingMachineCategoryAsync()
        {
            packingMachineCategoryHistories = new();
            var data = await XhsContext!.PackingMachineCategoryHistories.Include(p => p.Category).Include(p => p.PackingMachine)
                .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable)
                .OrderByDescending(c => c.Id).AsNoTracking().ToListAsync();

            foreach (var item in data)
            {
                PackingMachineCategoryHistory packing = new()
                {
                    Id = item.Id,
                    Category = item.Category,
                    PackingMachine = item.PackingMachine,
                    Category_Id = item.Category_Id,
                    PackingMachine_Id = item.PackingMachine_Id,
                    BeginDate = item.BeginDate,
                    EndDate = item.EndDate,
                    CreatedDate = item.CreatedDate,
                    ModifiedDate = item.ModifiedDate
                };

                packingMachineCategoryHistories.Add(packing);
            }

            return packingMachineCategoryHistories;
        }
        private async Task<List<Category>> GetCategoryAsync()
        {
            Categories = new();
            Categories = await XhsContext!.Category.Where(x => x.Status == (byte)HTStatusCodeEnum.Stable).AsNoTracking().ToListAsync();

            return Categories;
        }

        private async Task<List<Data.Entities.PackingMachine>> GetPackingMachineAsync()
        {
            packingMachines = new();
            packingMachines = await XhsContext!.PackingMachines.Where(x => x.Status == (byte)HTStatusCodeEnum.Stable && x.PackingType == 0).AsNoTracking().ToListAsync();
            return packingMachines;
        }

        private void RowClickEvent(TableRowClickEventArgs<PackingMachineCategoryHistory> tableRowClickEventArgs)
        {
            Input = new();
        }
    }
}

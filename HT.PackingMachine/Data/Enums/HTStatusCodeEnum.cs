namespace HT.PackingMachine.Data.Enums
{
    public enum HTStatusCodeEnum
    {
        /// <summary>
        /// Mới tạo
        /// </summary>
        CreateNew = 10,
        /// <summary>
        /// Đưa vào sử dụng
        /// </summary>
        Stable = 20,
        /// <summary>
        /// Hoàn thành
        /// </summary>
        Complete = 203,
        /// <summary>
        /// Không sử dụng
        /// </summary>
        NotUse = 200,
        /// <summary>
        /// Xóa
        /// </summary>
        Delete = 211
    }

    public enum HTPackingTypeEnum
    {
        BAO = 0,
        /// <summary>
        /// Xả XMR
        /// </summary>
        ROI = 1,
        /// <summary>
        /// Cửa đóng bao JUMBO
        /// </summary>
        // JUMBO = 2,
        /// <summary>
        /// Cửa đóng SLING
        /// </summary>
        SLING = 3,
        /// <summary>
        /// Chỉ cân và tính hàng, không tham gia liên động: cân JUMbo, cân SLING
        /// </summary>
        WEIGHT = 4
    }
}

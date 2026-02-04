/* =====================================================
   WAREHOUSE MANAGEMENT SYSTEM - DATABASE SCHEMA
   Fixed version with correct table references
===================================================== */

/* =====================================================
   TABLE: Suppliers
   Ý nghĩa: Nhà cung cấp
===================================================== */
CREATE TABLE Suppliers (
    id INT IDENTITY PRIMARY KEY,           -- ID nhà cung cấp
    code NVARCHAR(100) NOT NULL,
    type NVARCHAR(20) NOT NULL,            -- Cá nhân / Doanh nghiệp
    name NVARCHAR(150) NOT NULL,           -- Tên NCC

    contactPerson NVARCHAR(100),           -- Người liên hệ
    title NVARCHAR(50),                    -- Chức danh

    phone VARCHAR(20),
    email VARCHAR(100),

    role NVARCHAR(50),                     -- Vai trò NCC
    citizenId VARCHAR(20),                 -- CCCD (nếu cá nhân)

    address NVARCHAR(255),
    status NVARCHAR(50) NOT NULL,
    createdTime DATETIME DEFAULT GETDATE()
);
GO

/* =====================================================
   TABLE: Materials
   Ý nghĩa: Nguyên liệu / hàng hóa
===================================================== */
CREATE TABLE Materials (
    id INT IDENTITY PRIMARY KEY,           -- ID nguyên liệu
    code NVARCHAR(100) NOT NULL,
    name NVARCHAR(150) NOT NULL,
    categoryId INT NOT NULL,
    unitId INT NOT NULL,
    supplierId INT NOT NULL,
    stockQuantity DECIMAL(18,2) DEFAULT 0,
    note NVARCHAR(255),
    status NVARCHAR(50),
    createdTime DATETIME DEFAULT GETDATE(),

    CONSTRAINT FKMaterialSupplier
        FOREIGN KEY (supplierId) REFERENCES Suppliers(id)
);
GO

/* =====================================================
   TABLE: Warehouse
   Ý nghĩa: Kho hàng
===================================================== */
CREATE TABLE Warehouse (
    id INT IDENTITY PRIMARY KEY,
    code NVARCHAR(100) NOT NULL,
    name NVARCHAR(100) NOT NULL,
    typeId INT NOT NULL,

    address NVARCHAR(255),
    area DECIMAL(10,2),

    managerName NVARCHAR(100),
    managerPhone VARCHAR(20),

    status NVARCHAR(50),
    note NVARCHAR(255),
    createdTime DATETIME DEFAULT GETDATE()
);
GO

/* =====================================================
   TABLE: Inventory
   Ý nghĩa: Tồn kho theo kho và nguyên liệu
===================================================== */
CREATE TABLE Inventory (
    id INT IDENTITY PRIMARY KEY,

    warehouseId INT NOT NULL,
    materialId INT NOT NULL,

    quantity DECIMAL(18,2) NOT NULL,
    updatedDate DATETIME DEFAULT GETDATE(),

    CONSTRAINT UQInventory UNIQUE (warehouseId, materialId),

    CONSTRAINT FKInventoryWarehouse
        FOREIGN KEY (warehouseId) REFERENCES Warehouse(id),
    CONSTRAINT FKInventoryMaterial
        FOREIGN KEY (materialId) REFERENCES Materials(id)
);
GO

/* =====================================================
   TABLE: ImportReceipt
   Ý nghĩa: Phiếu nhập kho
===================================================== */
CREATE TABLE ImportReceipt (
    id INT IDENTITY PRIMARY KEY,
    code NVARCHAR(100) NOT NULL,
    receiptNumber NVARCHAR(30) NOT NULL UNIQUE,   -- Số phiếu nhập
    importTime DATETIME NOT NULL,                 -- Ngày lập phiếu

    supplierId INT NOT NULL,                      -- Nhà cung cấp
    warehouseId INT NOT NULL,                     -- Kho nhận

    supplierInvoiceNo NVARCHAR(50),               -- Số hóa đơn NCC
    documentNo NVARCHAR(50),                      -- Số chứng từ

    totalAmount DECIMAL(18,0),                    -- Tổng tiền
    status NVARCHAR(50) NOT NULL,                 -- Chờ xác nhận / Đã xác nhận

    createdBy NVARCHAR(100),
    approvedBy NVARCHAR(100),
    approvedAt DATETIME,

    note NVARCHAR(255),
    createdAt DATETIME DEFAULT GETDATE(),

    CONSTRAINT FKImportSupplier
        FOREIGN KEY (supplierId) REFERENCES Suppliers(id),
    CONSTRAINT FKImportWarehouse
        FOREIGN KEY (warehouseId) REFERENCES Warehouse(id)
);
GO

/* =====================================================
   TABLE: ImportReceiptDetail
   Ý nghĩa: Chi tiết phiếu nhập
===================================================== */
CREATE TABLE ImportReceiptDetail (
    id INT IDENTITY PRIMARY KEY,

    importReceiptId INT NOT NULL,
    materialId INT NOT NULL,
    unitId INT NOT NULL,

    quantity DECIMAL(18,2) NOT NULL,
    unitPrice DECIMAL(18,0) NOT NULL,
    amount AS (quantity * unitPrice),

    note NVARCHAR(255),

    CONSTRAINT FKIRDReceipt
        FOREIGN KEY (importReceiptId) REFERENCES ImportReceipt(id),
    CONSTRAINT FKIRDMaterial
        FOREIGN KEY (materialId) REFERENCES Materials(id),

    CONSTRAINT UQImportReceiptDetail
        UNIQUE (importReceiptId, materialId)
);
GO

/* =====================================================
   TABLE: ExportReceipt
   Ý nghĩa: Phiếu xuất kho
===================================================== */
CREATE TABLE ExportReceipt (
    id INT IDENTITY PRIMARY KEY,
    code NVARCHAR(100) NOT NULL,
    receiptNumber NVARCHAR(30) NOT NULL UNIQUE,   -- Số phiếu xuất
    exportDate DATETIME NOT NULL,

    warehouseId INT NOT NULL,
    receiverName NVARCHAR(150) NOT NULL,          -- Người nhận
    reason NVARCHAR(255) NOT NULL,

    documentNo NVARCHAR(50),
    totalAmount DECIMAL(18,0),

    status NVARCHAR(50) NOT NULL,
    createdBy NVARCHAR(100),
    approvedBy NVARCHAR(100),
    approvedAt DATETIME,

    note NVARCHAR(255),
    createdAt DATETIME DEFAULT GETDATE(),

    CONSTRAINT FKExportWarehouse
        FOREIGN KEY (warehouseId) REFERENCES Warehouse(id)
);
GO

/* =====================================================
   TABLE: ExportReceiptDetail
   Ý nghĩa: Chi tiết phiếu xuất
===================================================== */
CREATE TABLE ExportReceiptDetail (
    id INT IDENTITY PRIMARY KEY,

    exportReceiptId INT NOT NULL,
    materialId INT NOT NULL,
    unitId INT NOT NULL,

    quantity DECIMAL(18,2) NOT NULL,
    unitPrice DECIMAL(18,0),
    amount AS (quantity * unitPrice),

    note NVARCHAR(255),

    CONSTRAINT FKERDReceipt
        FOREIGN KEY (exportReceiptId) REFERENCES ExportReceipt(id),
    CONSTRAINT FKERDMaterial
        FOREIGN KEY (materialId) REFERENCES Materials(id),

    CONSTRAINT UQExportReceiptDetail
        UNIQUE (exportReceiptId, materialId)
);
GO

/* =====================================================
   TABLE: StockCheck
   Ý nghĩa: Kiểm kê kho
===================================================== */
CREATE TABLE StockCheck (
    id INT IDENTITY PRIMARY KEY,

    warehouseId INT NOT NULL,
    checkTime DATETIME NOT NULL,
    checkedBy NVARCHAR(100),
    note NVARCHAR(255),
    createdTime DATETIME DEFAULT GETDATE(),

    CONSTRAINT FKStockCheckWarehouse
        FOREIGN KEY (warehouseId) REFERENCES Warehouse(id)
);
GO

/* =====================================================
   TABLE: StockCheckDetail
   Ý nghĩa: Chi tiết kiểm kê
===================================================== */
CREATE TABLE StockCheckDetail (
    id INT IDENTITY PRIMARY KEY,

    stockCheckId INT NOT NULL,
    materialId INT NOT NULL,

    systemQuantity DECIMAL(18,2) NOT NULL,
    actualQuantity DECIMAL(18,2) NOT NULL,
    difference AS (actualQuantity - systemQuantity),

    CONSTRAINT FKScdCheck
        FOREIGN KEY (stockCheckId) REFERENCES StockCheck(id),
    CONSTRAINT FKScdMaterial
        FOREIGN KEY (materialId) REFERENCES Materials(id)
);
GO

/* =====================================================
   TABLE: AppUser
   Ý nghĩa: Người dùng hệ thống
===================================================== */
CREATE TABLE AppUser (
    id INT IDENTITY PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    role NVARCHAR(50) NOT NULL
);
GO

/* =====================================================
   SUMMARY OF FIXES:

   1. Fixed all foreign key references to use correct table names
   2. Fixed constraint naming consistency (PascalCase)
   3. Removed trailing commas in CREATE TABLE statements
   4. Ensured all table names match their references

   Tables: Suppliers, Materials, Warehouse, Inventory,
           ImportReceipt, ImportReceiptDetail,
           ExportReceipt, ExportReceiptDetail,
           StockCheck, StockCheckDetail, AppUser
===================================================== */

/* =====================================================
   SAMPLE DATA - 5 RECORDS PER TABLE
   Context: QUÁN ĂN / NHÀ HÀNG - KHO NGUYÊN LIỆU THỰC PHẨM
===================================================== */

-- Insert Suppliers (Nhà cung cấp thực phẩm)
INSERT INTO Suppliers (code, type, name, contactPerson, title, phone, email, role, citizenId, address, status, createdTime) VALUES
(N'SUP001', N'Doanh nghiệp', N'Công ty TNHH Thực phẩm Sạch Việt', N'Nguyễn Văn An', N'Giám đốc', '0901234567', 'an.nguyen@thucphamsach.com', N'Nhà cung cấp rau củ', NULL, N'123 Đường Lê Lợi, Quận 1, TP.HCM', N'Hoạt động', '2024-01-15'),
(N'SUP002', N'Doanh nghiệp', N'Công ty CP Thực phẩm 3F Việt Nam', N'Trần Thị Bình', N'Trưởng phòng kinh doanh', '0902345678', 'binh.tran@3fvietnam.com', N'Nhà cung cấp thịt', NULL, N'456 Đường Nguyễn Huệ, Quận 5, TP.HCM', N'Hoạt động', '2024-01-16'),
(N'SUP003', N'Cá nhân', N'Nguyễn Minh Cường', N'Nguyễn Minh Cường', N'Chủ vựa hải sản', '0903456789', 'cuong.nguyen@gmail.com', N'Nhà cung cấp hải sản', '079123456789', N'789 Chợ Bình Điền, Quận 8, TP.HCM', N'Hoạt động', '2024-01-17'),
(N'SUP004', N'Doanh nghiệp', N'Công ty TNHH Gia vị Á Đông', N'Lê Văn Dũng', N'Phó giám đốc', '0904567890', 'dung.le@giaviadong.com', N'Nhà cung cấp gia vị', NULL, N'321 Đường Phan Xích Long, Phú Nhuận, TP.HCM', N'Hoạt động', '2024-01-18'),
(N'SUP005', N'Doanh nghiệp', N'Công ty CP Đồ uống Tân Hiệp Phát', N'Phạm Thị Em', N'Giám đốc khu vực', '0905678901', 'em.pham@thp.com.vn', N'Nhà cung cấp đồ uống', NULL, N'654 Đường Cách Mạng Tháng 8, Quận 3, TP.HCM', N'Hoạt động', '2024-01-19');
GO

-- Insert Materials (Nguyên liệu thực phẩm)
INSERT INTO Materials (code, name, categoryId, unitId, supplierId, stockQuantity, note, status, createdTime) VALUES
(N'MAT001', N'Thịt bò Úc', 1, 1, 2, 50.00, N'Thịt bò nhập khẩu loại 1', N'Đang kinh doanh', '2024-01-20'),
(N'MAT002', N'Rau cải ngọt', 2, 2, 1, 30.00, N'Rau sạch VietGap', N'Đang kinh doanh', '2024-01-21'),
(N'MAT003', N'Tôm sú', 3, 1, 3, 25.00, N'Tôm sú size 20-30 con/kg', N'Đang kinh doanh', '2024-01-22'),
(N'MAT004', N'Nước mắm Nam Ngư', 4, 3, 4, 100.00, N'Nước mắm 40 độ đạm', N'Đang kinh doanh', '2024-01-23'),
(N'MAT005', N'Nước ngọt Coca Cola', 5, 4, 5, 200.00, N'Lon 330ml', N'Đang kinh doanh', '2024-01-24');
GO

-- Insert Warehouse (Kho thực phẩm)
INSERT INTO Warehouse (code, name, typeId, address, area, managerName, managerPhone, status, note, createdTime) VALUES
(N'WH001', N'Kho lạnh chính', 1, N'100 Đường Tân Sơn Nhì, Tân Phú, TP.HCM', 200.00, N'Hoàng Văn Khoa', '0911111111', N'Hoạt động', N'Kho lạnh -18°C', '2024-01-10'),
(N'WH002', N'Kho mát rau củ', 2, N'100 Đường Tân Sơn Nhì, Tân Phú, TP.HCM', 150.00, N'Đỗ Thị Lan', '0922222222', N'Hoạt động', N'Kho mát 2-8°C', '2024-01-11'),
(N'WH003', N'Kho khô', 1, N'100 Đường Tân Sơn Nhì, Tân Phú, TP.HCM', 100.00, N'Vũ Văn Nam', '0933333333', N'Hoạt động', N'Kho gia vị và đồ khô', '2024-01-12'),
(N'WH004', N'Kho đồ uống', 2, N'102 Đường Tân Sơn Nhì, Tân Phú, TP.HCM', 80.00, N'Bùi Thị Oanh', '0944444444', N'Hoạt động', N'Kho nước uống', '2024-01-13'),
(N'WH005', N'Kho dự trữ', 3, N'104 Đường Tân Sơn Nhì, Tân Phú, TP.HCM', 120.00, N'Phan Văn Phong', '0955555555', N'Hoạt động', N'Kho dự phòng', '2024-01-14');
GO

-- Insert Inventory (Tồn kho)
INSERT INTO Inventory (warehouseId, materialId, quantity, updatedDate) VALUES
(1, 1, 30.00, '2024-02-01'),
(2, 2, 20.00, '2024-02-01'),
(1, 3, 15.00, '2024-02-01'),
(3, 4, 50.00, '2024-02-01'),
(4, 5, 150.00, '2024-02-01');
GO

-- Insert ImportReceipt (Phiếu nhập kho)
INSERT INTO ImportReceipt (code, receiptNumber, importTime, supplierId, warehouseId, supplierInvoiceNo, documentNo, totalAmount, status, createdBy, approvedBy, approvedAt, note, createdAt) VALUES
(N'IMP001', N'PN-2024-001', '2024-02-01 06:00:00', 1, 2, N'HD-RAU-001', N'CT-001', 3000000, N'Đã xác nhận', N'Thủ kho', N'Quản lý', '2024-02-01 07:00:00', N'Nhập rau củ tươi sáng', '2024-02-01 06:00:00'),
(N'IMP002', N'PN-2024-002', '2024-02-02 05:30:00', 2, 1, N'HD-THIT-001', N'CT-002', 15000000, N'Đã xác nhận', N'Thủ kho', N'Quản lý', '2024-02-02 07:00:00', N'Nhập thịt bò tươi', '2024-02-02 05:30:00'),
(N'IMP003', N'PN-2024-003', '2024-02-03 05:00:00', 3, 1, N'HD-HS-001', N'CT-003', 7500000, N'Đã xác nhận', N'Thủ kho', N'Quản lý', '2024-02-03 06:30:00', N'Nhập hải sản tươi sống', '2024-02-03 05:00:00'),
(N'IMP004', N'PN-2024-004', '2024-02-04 08:00:00', 4, 3, N'HD-GV-001', N'CT-004', 5000000, N'Chờ xác nhận', N'Thủ kho', NULL, NULL, N'Nhập gia vị các loại', '2024-02-04 08:00:00'),
(N'IMP005', N'PN-2024-005', '2024-02-05 09:00:00', 5, 4, N'HD-NU-001', N'CT-005', 8000000, N'Đã xác nhận', N'Thủ kho', N'Quản lý', '2024-02-05 10:00:00', N'Nhập nước uống các loại', '2024-02-05 09:00:00');
GO

-- Insert ImportReceiptDetail (Chi tiết phiếu nhập)
INSERT INTO ImportReceiptDetail (importReceiptId, materialId, unitId, quantity, unitPrice, note) VALUES
(1, 2, 2, 30.00, 100000, N'Rau cải ngọt tươi'),
(2, 1, 1, 50.00, 300000, N'Thịt bò Úc nhập khẩu'),
(3, 3, 1, 25.00, 300000, N'Tôm sú tươi sống'),
(4, 4, 3, 100.00, 50000, N'Nước mắm Nam Ngư 650ml'),
(5, 5, 4, 200.00, 40000, N'Coca Cola lon 330ml');
GO

-- Insert ExportReceipt (Phiếu xuất kho - cho bếp)
INSERT INTO ExportReceipt (code, receiptNumber, exportDate, warehouseId, receiverName, reason, documentNo, totalAmount, status, createdBy, approvedBy, approvedAt, note, createdAt) VALUES
(N'EXP001', N'PX-2024-001', '2024-02-06 10:00:00', 2, N'Bếp tầng 1', N'Xuất nguyên liệu cho ca trưa', N'CT-PX-001', 1000000, N'Đã xác nhận', N'Thủ kho', N'Quản lý', '2024-02-06 10:30:00', N'Xuất rau cho ca trưa', '2024-02-06 10:00:00'),
(N'EXP002', N'PX-2024-002', '2024-02-07 10:00:00', 1, N'Bếp tầng 1', N'Xuất nguyên liệu cho ca trưa', N'CT-PX-002', 6000000, N'Đã xác nhận', N'Thủ kho', N'Quản lý', '2024-02-07 10:30:00', N'Xuất thịt bò cho ca trưa', '2024-02-07 10:00:00'),
(N'EXP003', N'PX-2024-003', '2024-02-08 10:00:00', 1, N'Bếp tầng 2', N'Xuất nguyên liệu cho tiệc', N'CT-PX-003', 3000000, N'Đã xác nhận', N'Thủ kho', N'Quản lý', '2024-02-08 10:30:00', N'Xuất hải sản cho tiệc', '2024-02-08 10:00:00'),
(N'EXP004', N'PX-2024-004', '2024-02-09 15:00:00', 3, N'Bếp tầng 1', N'Bổ sung gia vị', N'CT-PX-004', 500000, N'Chờ xác nhận', N'Thủ kho', NULL, NULL, N'Xuất nước mắm', '2024-02-09 15:00:00'),
(N'EXP005', N'PX-2024-005', '2024-02-10 11:00:00', 4, N'Quầy bar', N'Bổ sung nước uống', N'CT-PX-005', 2000000, N'Đã xác nhận', N'Thủ kho', N'Quản lý', '2024-02-10 11:30:00', N'Xuất nước ngọt', '2024-02-10 11:00:00');
GO

-- Insert ExportReceiptDetail (Chi tiết phiếu xuất)
INSERT INTO ExportReceiptDetail (exportReceiptId, materialId, unitId, quantity, unitPrice, note) VALUES
(1, 2, 2, 10.00, 100000, N'Rau cải ngọt'),
(2, 1, 1, 20.00, 300000, N'Thịt bò Úc'),
(3, 3, 1, 10.00, 300000, N'Tôm sú'),
(4, 4, 3, 10.00, 50000, N'Nước mắm Nam Ngư'),
(5, 5, 4, 50.00, 40000, N'Coca Cola lon');
GO

-- Insert StockCheck (Kiểm kê kho)
INSERT INTO StockCheck (warehouseId, checkTime, checkedBy, note, createdTime) VALUES
(1, '2024-02-11 07:00:00', N'Hoàng Văn Khoa', N'Kiểm kê kho lạnh định kỳ', '2024-02-11 07:00:00'),
(2, '2024-02-12 07:00:00', N'Đỗ Thị Lan', N'Kiểm kê kho rau củ', '2024-02-12 07:00:00'),
(3, '2024-02-13 08:00:00', N'Vũ Văn Nam', N'Kiểm kê kho khô hàng tháng', '2024-02-13 08:00:00'),
(4, '2024-02-14 09:00:00', N'Bùi Thị Oanh', N'Kiểm kê kho đồ uống', '2024-02-14 09:00:00'),
(1, '2024-02-15 07:00:00', N'Hoàng Văn Khoa', N'Kiểm kê sau ca xuất hàng', '2024-02-15 07:00:00');
GO

-- Insert StockCheckDetail (Chi tiết kiểm kê)
INSERT INTO StockCheckDetail (stockCheckId, materialId, systemQuantity, actualQuantity) VALUES
(1, 1, 30.00, 29.50),
(1, 3, 15.00, 14.80),
(2, 2, 20.00, 19.50),
(3, 4, 50.00, 48.00),
(4, 5, 150.00, 148.00);
GO

-- Insert AppUser (Người dùng hệ thống quản lý kho)
INSERT INTO AppUser (username, password, role) VALUES
('manager', '$2a$11$hashed_password_manager', N'Quản lý kho'),
('staff01', '$2a$11$hashed_password_staff01', N'Nhân viên kho'),
('staff02', '$2a$11$hashed_password_staff02', N'Nhân viên kho'),
('staff03', '$2a$11$hashed_password_staff03', N'Nhân viên kho'),
('staff04', '$2a$11$hashed_password_staff04', N'Nhân viên kho');
GO

/* =====================================================
   DATA INSERTION COMPLETED - QUÁN ĂN / NHÀ HÀNG
   - 5 Suppliers (Nhà cung cấp thực phẩm)
   - 5 Materials (Nguyên liệu: thịt, rau, hải sản, gia vị, đồ uống)
   - 5 Warehouses (Kho lạnh, kho mát, kho khô, kho đồ uống)
   - 5 Inventory records (Tồn kho)
   - 5 Import Receipts with Details (Phiếu nhập hàng sáng)
   - 5 Export Receipts with Details (Phiếu xuất cho bếp)
   - 5 Stock Checks with Details (Kiểm kê định kỳ)
   - 5 App Users (1 Quản lý kho + 4 Nhân viên kho)
===================================================== */

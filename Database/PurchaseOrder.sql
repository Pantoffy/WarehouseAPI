/* =====================================================
   TABLE: PurchaseOrder
   Ý nghĩa: Đơn đặt hàng mua (từ nhà cung cấp)
===================================================== */
CREATE TABLE PurchaseOrder (
    id INT IDENTITY PRIMARY KEY,
    code NVARCHAR(100) NOT NULL,
    poNumber NVARCHAR(30) NOT NULL UNIQUE,        -- Số đơn đặt hàng
    orderDate DATETIME NOT NULL,                   -- Ngày đặt hàng

    supplierId INT NOT NULL,                       -- Nhà cung cấp
    expectedDeliveryDate DATETIME,                 -- Ngày giao hàng dự kiến

    totalAmount DECIMAL(18,0),                     -- Tổng tiền
    status NVARCHAR(50) NOT NULL,                  -- Chờ duyệt / Đã duyệt / Đã nhận

    createdBy NVARCHAR(100),
    approvedBy NVARCHAR(100),
    approvedAt DATETIME,

    note NVARCHAR(255),
    createdAt DATETIME DEFAULT GETDATE(),

    CONSTRAINT FKPurchaseOrderSupplier
        FOREIGN KEY (supplierId) REFERENCES Suppliers(id)
);
GO

/* =====================================================
   TABLE: PurchaseOrderDetail
   Ý nghĩa: Chi tiết đơn đặt hàng
===================================================== */
CREATE TABLE PurchaseOrderDetail (
    id INT IDENTITY PRIMARY KEY,

    purchaseOrderId INT NOT NULL,
    materialId INT NOT NULL,
    unitId INT NOT NULL,

    quantity DECIMAL(18,2) NOT NULL,
    unitPrice DECIMAL(18,0) NOT NULL,
    amount AS (quantity * unitPrice),

    note NVARCHAR(255),

    CONSTRAINT FKPODPurchaseOrder
        FOREIGN KEY (purchaseOrderId) REFERENCES PurchaseOrder(id),
    CONSTRAINT FKPODMaterial
        FOREIGN KEY (materialId) REFERENCES Materials(id),

    CONSTRAINT UQPurchaseOrderDetail
        UNIQUE (purchaseOrderId, materialId)
);
GO

/* =====================================================
   SAMPLE DATA - 5 PURCHASE ORDERS
===================================================== */

-- Insert PurchaseOrder
INSERT INTO PurchaseOrder (code, poNumber, orderDate, supplierId, expectedDeliveryDate, totalAmount, status, createdBy, approvedBy, approvedAt, note, createdAt) VALUES
(N'PO001', N'PO-2024-001', '2024-01-25 08:00:00', 1, '2024-02-01 08:00:00', 3000000, N'Đã duyệt', N'Admin', N'Manager', '2024-01-25 10:00:00', N'Đặt rau củ tháng 2', '2024-01-25 08:00:00'),
(N'PO002', N'PO-2024-002', '2024-01-26 09:00:00', 2, '2024-02-02 08:00:00', 15000000, N'Đã duyệt', N'Admin', N'Manager', '2024-01-26 11:00:00', N'Đặt thịt bò', '2024-01-26 09:00:00'),
(N'PO003', N'PO-2024-003', '2024-01-27 10:00:00', 3, '2024-02-03 08:00:00', 7500000, N'Đã duyệt', N'Admin', N'Manager', '2024-01-27 12:00:00', N'Đặt hải sản', '2024-01-27 10:00:00'),
(N'PO004', N'PO-2024-004', '2024-01-28 11:00:00', 4, '2024-02-04 08:00:00', 5000000, N'Chờ duyệt', N'Admin', NULL, NULL, N'Đặt gia vị', '2024-01-28 11:00:00'),
(N'PO005', N'PO-2024-005', '2024-01-29 12:00:00', 5, '2024-02-05 08:00:00', 8000000, N'Đã duyệt', N'Admin', N'Manager', '2024-01-29 14:00:00', N'Đặt nước uống', '2024-01-29 12:00:00');
GO

-- Insert PurchaseOrderDetail
INSERT INTO PurchaseOrderDetail (purchaseOrderId, materialId, unitId, quantity, unitPrice, note) VALUES
(1, 2, 2, 30.00, 100000, N'Rau cải ngọt tươi'),
(2, 1, 1, 50.00, 300000, N'Thịt bò Úc nhập khẩu'),
(3, 3, 1, 25.00, 300000, N'Tôm sú tươi sống'),
(4, 4, 3, 100.00, 50000, N'Nước mắm Nam Ngư 650ml'),
(5, 5, 4, 200.00, 40000, N'Coca Cola lon 330ml');
GO

/* =====================================================
   COMPLETED: PurchaseOrder & PurchaseOrderDetail
   
   - PurchaseOrder: Đơn đặt hàng từ nhà cung cấp
   - PurchaseOrderDetail: Chi tiết nguyên liệu đặt
   - Status: Chờ duyệt → Đã duyệt → Đã nhận (chuyển sang ImportReceipt)
   
   Luồng: PurchaseOrder → ImportReceipt → Inventory
===================================================== */

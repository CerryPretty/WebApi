using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class WebApiECommerceDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCatalogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCatalogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ManagerId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    MasterId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ProblemDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ManagerComments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MasterComments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ClientDisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_OrderStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "OrderStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EventDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UserDisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderLogs_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderServices",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderServices", x => new { x.OrderId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_OrderServices_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderServices_ServiceCatalogs_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "ServiceCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "Id", "StatusName" },
                values: new object[,]
                {
                    { 1, "Обработка заказа" },
                    { 2, "Производятся ремонтные услуги" },
                    { 3, "Готов к выдаче" },
                    { 4, "Оплачено" },
                    { 5, "Отказ" }
                });

            migrationBuilder.InsertData(
                table: "ServiceCatalogs",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "ImageUrl", "Price", "ServiceName", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Механический ремонт", new DateTime(2023, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Полная разборка, дефектовка, замена изношенных деталей и сборка двигателя Д-240 с гарантией качества.", "/images/service_diesel_engine.jpg", 15000m, "Капитальный ремонт дизельного двигателя Д-240", null },
                    { 2, "Механический ремонт", new DateTime(2023, 2, 10, 10, 0, 0, 0, DateTimeKind.Utc), "Комплексная проверка гидравлической системы, включая тестирование давления, поиск утечек и анализ состояния масла.", "/images/service_hydraulic_diag.jpg", 1200m, "Диагностика гидравлической системы пресса", null },
                    { 3, "Производственные услуги", new DateTime(2023, 3, 1, 11, 0, 0, 0, DateTimeKind.Utc), "Профессиональная заточка всех видов металлообрабатывающего инструмента для повышения точности и срока службы.", "/images/service_tool_sharpening.jpg", 300m, "Заточка инструмента для металлообработки", null },
                    { 4, "Механический ремонт", new DateTime(2023, 4, 20, 12, 0, 0, 0, DateTimeKind.Utc), "Дефектовка, ремонт или полная замена изношенных элементов редуктора крана РДК-25 с последующей проверкой на стенде.", "/images/service_crane_reducer.jpg", 8000m, "Ремонт редуктора крана РДК-25", null },
                    { 5, "Производственные услуги", new DateTime(2023, 5, 5, 13, 0, 0, 0, DateTimeKind.Utc), "Визуальный осмотр, инструментальный контроль и диагностика для выявления дефектов и определения объема ремонтных работ.", "/images/service_defectoscopy.jpg", 700m, "Дефектовка узлов и агрегатов", null },
                    { 6, "Производственные услуги", new DateTime(2023, 6, 12, 14, 0, 0, 0, DateTimeKind.Utc), "Восстановление целостности металлических конструкций с использованием различных методов сварки.", "/images/service_welding.jpg", 2500m, "Сварочные работы по восстановлению конструкции", null },
                    { 7, "Механический ремонт", new DateTime(2023, 7, 1, 15, 0, 0, 0, DateTimeKind.Utc), "Регулярное техническое обслуживание, чистка, смазка и замена изношенных элементов насосов для предотвращения поломок.", "/images/service_pump_maintenance.jpg", 1800m, "Обслуживание насосного оборудования", null },
                    { 8, "Производственные услуги", new DateTime(2023, 8, 10, 16, 0, 0, 0, DateTimeKind.Utc), "Динамическая и статическая балансировка вращающихся частей оборудования для снижения вибрации и увеличения срока службы.", "/images/service_balancing.jpg", 3200m, "Балансировка роторов и валов", null },
                    { 9, "Электрооборудование", new DateTime(2023, 9, 1, 9, 30, 0, 0, DateTimeKind.Utc), "Диагностика, ремонт электронных компонентов и программного обеспечения контроллеров Siemens S7.", "/images/service_siemens_plc.jpg", 6000m, "Ремонт промышленного контроллера Siemens S7", null },
                    { 10, "Электрооборудование", new DateTime(2023, 10, 15, 10, 30, 0, 0, DateTimeKind.Utc), "Выверка и настройка датчиков давления в соответствии с эталонными стандартами.", "/images/service_pressure_sensor.jpg", 900m, "Калибровка датчиков давления", null },
                    { 11, "Электрооборудование", new DateTime(2023, 11, 1, 11, 30, 0, 0, DateTimeKind.Utc), "Комплексное обслуживание систем автоматизации, включая проверку проводки, настройку датчиков и обновление ПО.", "/images/service_automation.jpg", 4000m, "Обслуживание систем автоматизации", null },
                    { 12, "Запчасти и материалы", new DateTime(2023, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc), "Высококачественный шарикоподшипник с двухсторонним металлическим уплотнением для различных механизмов.", "/images/part_bearing.jpg", 150m, "Подшипник скольжения 6205 ZZ", null },
                    { 13, "Запчасти и материалы", new DateTime(2023, 1, 1, 8, 5, 0, 0, DateTimeKind.Utc), "Высокопрочная гидравлическая манжета для герметизации соединений в условиях высокого давления.", "/images/part_seal.jpg", 80m, "Уплотнительная манжета 80x100x10", null },
                    { 14, "Запчасти и материалы", new DateTime(2023, 1, 1, 8, 10, 0, 0, DateTimeKind.Utc), "Полный набор высококачественных прокладок, необходимый для капитального ремонта дизельного двигателя Д-240.", "/images/part_gasket_set.jpg", 1200m, "Комплект прокладок для двигателя Д-240", null },
                    { 15, "Запчасти и материалы", new DateTime(2023, 1, 1, 8, 15, 0, 0, DateTimeKind.Utc), "Медный силовой кабель с пониженным дымо- и газовыделением, огнестойкий, для прокладки внутри помещений.", "/images/part_cable.jpg", 250m, "Кабель силовой ВВГнг-LS 3х2.5", null },
                    { 16, "Промышленное оборудование", new DateTime(2023, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc), "Восстановленный универсальный токарно-винторезный станок 1К62, полностью готов к эксплуатации, после капитального ремонта.", "/images/equipment_lathe.jpg", 85000m, "Токарный станок 1К62 (б/у)", new DateTime(2023, 3, 20, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, "Промышленное оборудование", new DateTime(2023, 2, 1, 9, 0, 0, 0, DateTimeKind.Utc), "Профессиональный сварочный аппарат для аргонодуговой сварки (TIG) и ручной дуговой сварки (MMA), с полным комплектом оснастки.", "/images/equipment_welder.jpg", 32000m, "Сварочный аппарат КЕМППИ MasterTig", null }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "ClientDisplayName", "ClientId", "CompletionDate", "Cost", "CreatedDate", "ManagerComments", "ManagerId", "MasterComments", "MasterId", "OrderNumber", "ProblemDescription", "StartDate", "StatusId" },
                values: new object[,]
                {
                    { 1, "ООО 'МеталлПрофиль'", "client_metalprofile_id", null, 15000m, new DateTime(2024, 5, 2, 10, 30, 0, 0, DateTimeKind.Utc), "Необходим полный капитальный ремонт, согласовано с клиентом.", "manager_john_doe_id", null, "master_alex_smith_id", "ORD01", "Дизельный двигатель Д-240: Снижение мощности, повышенный расход топлива", new DateTime(2024, 5, 3, 9, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, "ЧУП 'ГидроПрессСервис'", "client_hydropress_id", null, 1200m, new DateTime(2024, 5, 11, 12, 0, 0, 0, DateTimeKind.Utc), "Диагностика показала необходимость замены уплотнений.", "manager_jane_doe_id", "Начаты работы по замене гидравлических уплотнений.", "master_bob_johnson_id", "ORD02", "Гидравлический пресс П3236: Неисправность гидравлической системы, утечка масла.", new DateTime(2024, 5, 12, 10, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 3, "ОАО 'СтройМаш'", "client_stroymash_id", new DateTime(2024, 5, 16, 15, 0, 0, 0, DateTimeKind.Utc), 300m, new DateTime(2024, 5, 16, 9, 0, 0, 0, DateTimeKind.Utc), "Заточка выполнена, инструмент готов к выдаче.", "manager_john_doe_id", "Заточка произведена по ГОСТ.", "master_alex_smith_id", "ORD03", "Токарный станок 1К62: Требуется заточка резцов.", new DateTime(2024, 5, 16, 11, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 4, "УП 'ПромКомплект'", "client_promkomplekt_id", null, 8000m, new DateTime(2024, 6, 6, 9, 15, 0, 0, DateTimeKind.Utc), "Ожидаем поступление запчастей для редуктора.", "manager_jane_doe_id", null, "master_bob_johnson_id", "ORD04", "Подъемный кран РДК-25: Износ редуктора, повышенный шум при работе.", null, 1 },
                    { 5, "КФХ 'АгроТех'", "client_agrotech_id", null, 700m, new DateTime(2024, 6, 21, 11, 0, 0, 0, DateTimeKind.Utc), null, "manager_john_doe_id", "Начата дефектовка подшипникового узла.", "master_alex_smith_id", "ORD05", "Насос центробежный НЦ-500: Неисправность, требуется дефектовка для определения причины.", new DateTime(2024, 6, 22, 9, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 6, "ИП Иванов И.И.", "client_ivanov_id", null, 2500m, new DateTime(2024, 6, 23, 10, 0, 0, 0, DateTimeKind.Utc), "Требуются сварочные работы и усиление рамы.", "manager_john_doe_id", null, "master_bob_johnson_id", "ORD06", "Металлоконструкция (Рама станка): Обнаружена трещина в несущей раме.", null, 1 },
                    { 7, "ОДО 'ЭлектроСистемы'", "client_electrosys_id", null, 6000m, new DateTime(2024, 6, 19, 14, 0, 0, 0, DateTimeKind.Utc), "Перепрошивка и тестирование контроллера.", "manager_jane_doe_id", "Выявлены программные ошибки, выполняется отладка.", "master_alex_smith_id", "ORD07", "Промышленный контроллер Siemens S7-1200: Сбой в работе, ошибки программы.", new DateTime(2024, 6, 20, 9, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 8, "ЗАО 'Автоматика'", "client_automatika_id", new DateTime(2024, 6, 13, 16, 0, 0, 0, DateTimeKind.Utc), 900m, new DateTime(2024, 6, 12, 11, 0, 0, 0, DateTimeKind.Utc), "Калибровка выполнена, датчик соответствует эталонным показаниям.", "manager_john_doe_id", "Проведена калибровка по всем точкам, отчет сформирован.", "master_bob_johnson_id", "ORD08", "Датчик давления Rosemount 3051: Показания неточные, требуется калибровка.", new DateTime(2024, 6, 13, 9, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 9, "СП 'РемСтройМаш'", "client_remstroymash_id", new DateTime(2024, 4, 15, 10, 0, 0, 0, DateTimeKind.Utc), 85000m, new DateTime(2024, 4, 12, 16, 0, 0, 0, DateTimeKind.Utc), "Договор купли-продажи подписан, оплата получена. Готов к отгрузке.", "manager_jane_doe_id", null, null, "ORD09", "Покупка восстановленного токарного станка 1К62.", null, 4 },
                    { 10, "Гомельский завод литья", "client_gomelcasting_id", null, 32000m, new DateTime(2024, 5, 4, 10, 0, 0, 0, DateTimeKind.Utc), "Оформляются документы на продажу, ждем предоплату.", "manager_john_doe_id", null, null, "ORD10", "Покупка сварочного аппарата КЕМППИ MasterTig 2350.", null, 1 }
                });

            migrationBuilder.InsertData(
                table: "OrderLogs",
                columns: new[] { "Id", "CustomerName", "EventDate", "EventDescription", "ItemCode", "ItemName", "OrderId", "OrderNumber", "UserDisplayName", "UserId" },
                values: new object[,]
                {
                    { 1, "ООО 'МеталлПрофиль'", new DateTime(2024, 5, 2, 10, 35, 0, 0, DateTimeKind.Utc), "Заказ создан. Зарегистрирована заявка на ремонт двигателя.", "SRV001", "Капитальный ремонт дизельного двигателя Д-240", 1, "ORD01", "Джон Доу (Менеджер)", "manager_john_doe_id" },
                    { 2, "ООО 'МеталлПрофиль'", new DateTime(2024, 5, 3, 9, 10, 0, 0, DateTimeKind.Utc), "Начаты работы по капитальному ремонту двигателя. Передан мастеру.", "SRV001", "Начало работ по ремонту Д-240", 1, "ORD01", "Алекс Смит (Мастер)", "master_alex_smith_id" },
                    { 3, "ЧУП 'ГидроПрессСервис'", new DateTime(2024, 5, 11, 12, 15, 0, 0, DateTimeKind.Utc), "Заказ создан. Зарегистрирована заявка на диагностику гидравлики.", "SRV002", "Диагностика гидравлической системы пресса", 2, "ORD02", "Джейн Доу (Менеджер)", "manager_jane_doe_id" },
                    { 4, "ЧУП 'ГидроПрессСервис'", new DateTime(2024, 5, 12, 10, 30, 0, 0, DateTimeKind.Utc), "Проведена дефектовка, выявлена необходимость замены уплотнений.", "SRV002", "Дефектовка гидравлической системы", 2, "ORD02", "Боб Джонсон (Мастер)", "master_bob_johnson_id" },
                    { 5, "ОАО 'СтройМаш'", new DateTime(2024, 5, 16, 15, 5, 0, 0, DateTimeKind.Utc), "Работы по заточке завершены. Заказ готов к выдаче.", "SRV003", "Заточка инструмента для металлообработки", 3, "ORD03", "Алекс Смит (Мастер)", "master_alex_smith_id" },
                    { 6, "УП 'ПромКомплект'", new DateTime(2024, 6, 6, 9, 30, 0, 0, DateTimeKind.Utc), "Заказ создан. Ожидание поставки запчастей для редуктора.", "SRV004", "Ремонт редуктора крана РДК-25", 4, "ORD04", "Джейн Доу (Менеджер)", "manager_jane_doe_id" },
                    { 7, "УП 'ПромКомплект'", new DateTime(2024, 6, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Запчасти для редуктора получены на склад.", "PART01", "Подшипники для редуктора", 4, "ORD04", "Джейн Доу (Менеджер)", "manager_jane_doe_id" },
                    { 8, "КФХ 'АгроТех'", new DateTime(2024, 6, 22, 9, 30, 0, 0, DateTimeKind.Utc), "Дефектовка узлов и агрегатов насоса завершена. Согласование стоимости ремонта с клиентом.", "SRV005", "Дефектовка насосного оборудования", 5, "ORD05", "Алекс Смит (Мастер)", "master_alex_smith_id" },
                    { 9, "ИП Иванов И.И.", new DateTime(2024, 6, 23, 10, 10, 0, 0, DateTimeKind.Utc), "Заказ создан. Требуется оценка масштаба сварочных работ.", "SRV006", "Сварочные работы по восстановлению конструкции", 6, "ORD06", "Джон Доу (Менеджер)", "manager_john_doe_id" },
                    { 10, "ОДО 'ЭлектроСистемы'", new DateTime(2024, 6, 21, 11, 0, 0, 0, DateTimeKind.Utc), "Программный ремонт контроллера Siemens S7 завершен, проведено тестирование.", "SRV009", "Ремонт промышленного контроллера Siemens S7", 7, "ORD07", "Алекс Смит (Мастер)", "master_alex_smith_id" },
                    { 11, "ЗАО 'Автоматика'", new DateTime(2024, 6, 13, 16, 15, 0, 0, DateTimeKind.Utc), "Калибровка датчика успешно завершена. Оформлены протоколы калибровки.", "SRV010", "Калибровка датчиков давления", 8, "ORD08", "Боб Джонсон (Мастер)", "master_bob_johnson_id" },
                    { 12, "СП 'РемСтройМаш'", new DateTime(2024, 4, 12, 16, 30, 0, 0, DateTimeKind.Utc), "Заказ продажи станка создан. Согласование условий.", "EQP001", "Токарный станок 1К62 (б/у)", 9, "ORD09", "Джейн Доу (Менеджер)", "manager_jane_doe_id" },
                    { 13, "СП 'РемСтройМаш'", new DateTime(2024, 4, 15, 10, 30, 0, 0, DateTimeKind.Utc), "Токарный станок 1К62 отгружен клиенту. Оплата получена в полном объеме.", "EQP001", "Отгрузка токарного станка 1К62", 9, "ORD09", "Джейн Доу (Менеджер)", "manager_jane_doe_id" },
                    { 14, "Гомельский завод литья", new DateTime(2024, 5, 4, 10, 15, 0, 0, DateTimeKind.Utc), "Заказ продажи сварочного аппарата создан. Выставлен счет на предоплату.", "EQP002", "Сварочный аппарат КЕМППИ MasterTig", 10, "ORD10", "Джон Доу (Менеджер)", "manager_john_doe_id" }
                });

            migrationBuilder.InsertData(
                table: "OrderServices",
                columns: new[] { "OrderId", "ServiceId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 1, 1, 15000m },
                    { 1, 14, 1, 1200m },
                    { 2, 2, 1, 1200m },
                    { 3, 3, 5, 300m },
                    { 4, 4, 1, 8000m },
                    { 5, 5, 1, 700m },
                    { 6, 6, 1, 2500m },
                    { 7, 9, 1, 6000m },
                    { 8, 10, 1, 900m },
                    { 9, 16, 1, 85000m },
                    { 10, 17, 1, 32000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderLogs_OrderId",
                table: "OrderLogs",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_StatusId",
                table: "Orders",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderServices_ServiceId",
                table: "OrderServices",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderLogs");

            migrationBuilder.DropTable(
                name: "OrderServices");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ServiceCatalogs");

            migrationBuilder.DropTable(
                name: "OrderStatuses");
        }
    }
}

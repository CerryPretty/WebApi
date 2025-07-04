using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization; 

namespace WebApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<ServiceCatalog> ServiceCatalogs { get; set; }
        public DbSet<OrderService> OrderServices { get; set; }
        public DbSet<OrderLog> OrderLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderService>()
                .HasKey(os => new { os.OrderId, os.ServiceId });

            modelBuilder.Entity<OrderService>()
                .HasOne(os => os.Order)
                .WithMany(o => o.OrderServices)
                .HasForeignKey(os => os.OrderId);

            modelBuilder.Entity<OrderService>()
                .HasOne(os => os.Service)
                .WithMany(s => s.OrderServices)
                .HasForeignKey(os => os.ServiceId);

            modelBuilder.Entity<OrderLog>()
                .HasOne(ol => ol.Order)
                .WithMany(o => o.OrderLogs)
                .HasForeignKey(ol => ol.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderStatus>().HasData(
                new OrderStatus { Id = 1, StatusName = "Обработка заказа" },
                new OrderStatus { Id = 2, StatusName = "Производятся ремонтные услуги" },
                new OrderStatus { Id = 3, StatusName = "Готов к выдаче" },
                new OrderStatus { Id = 4, StatusName = "Оплачено" },
                new OrderStatus { Id = 5, StatusName = "Отказ" }
            );

            modelBuilder.Entity<ServiceCatalog>().HasData(
                new ServiceCatalog
                {
                    Id = 1,
                    ServiceName = "Капитальный ремонт дизельного двигателя Д-240",
                    Price = 15000m,
                    Category = "Механический ремонт",
                    Description = "Полная разборка, дефектовка, замена изношенных деталей и сборка двигателя Д-240 с гарантией качества.",
                    ImageUrl = "/images/service_diesel_engine.jpg",
                    CreatedAt = DateTime.Parse("2023-01-15T09:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    UpdatedAt = null
                },
                new ServiceCatalog
                {
                    Id = 2,
                    ServiceName = "Диагностика гидравлической системы пресса",
                    Price = 1200m,
                    Category = "Механический ремонт",
                    Description = "Комплексная проверка гидравлической системы, включая тестирование давления, поиск утечек и анализ состояния масла.",
                    ImageUrl = "/images/service_hydraulic_diag.jpg",
                    CreatedAt = DateTime.Parse("2023-02-10T10:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    UpdatedAt = null
                },
                new ServiceCatalog
                {
                    Id = 3,
                    ServiceName = "Заточка инструмента для металлообработки",
                    Price = 300m,
                    Category = "Производственные услуги",
                    Description = "Профессиональная заточка всех видов металлообрабатывающего инструмента для повышения точности и срока службы.",
                    ImageUrl = "/images/service_tool_sharpening.jpg",
                    CreatedAt = DateTime.Parse("2023-03-01T11:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    UpdatedAt = null
                },
                new ServiceCatalog
                {
                    Id = 4,
                    ServiceName = "Ремонт редуктора крана РДК-25",
                    Price = 8000m,
                    Category = "Механический ремонт",
                    Description = "Дефектовка, ремонт или полная замена изношенных элементов редуктора крана РДК-25 с последующей проверкой на стенде.",
                    ImageUrl = "/images/service_crane_reducer.jpg",
                    CreatedAt = DateTime.Parse("2023-04-20T12:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    UpdatedAt = null
                },
                new ServiceCatalog
                {
                    Id = 5,
                    ServiceName = "Дефектовка узлов и агрегатов",
                    Price = 700m,
                    Category = "Производственные услуги",
                    Description = "Визуальный осмотр, инструментальный контроль и диагностика для выявления дефектов и определения объема ремонтных работ.",
                    ImageUrl = "/images/service_defectoscopy.jpg",
                    CreatedAt = DateTime.Parse("2023-05-05T13:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    UpdatedAt = null
                },
                new ServiceCatalog
                {
                    Id = 6,
                    ServiceName = "Сварочные работы по восстановлению конструкции",
                    Price = 2500m,
                    Category = "Производственные услуги",
                    Description = "Восстановление целостности металлических конструкций с использованием различных методов сварки.",
                    ImageUrl = "/images/service_welding.jpg",
                    CreatedAt = DateTime.Parse("2023-06-12T14:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    UpdatedAt = null
                },
                new ServiceCatalog
                {
                    Id = 7,
                    ServiceName = "Обслуживание насосного оборудования",
                    Price = 1800m,
                    Category = "Механический ремонт",
                    Description = "Регулярное техническое обслуживание, чистка, смазка и замена изношенных элементов насосов для предотвращения поломок.",
                    ImageUrl = "/images/service_pump_maintenance.jpg",
                    CreatedAt = DateTime.Parse("2023-07-01T15:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    UpdatedAt = null
                },
                new ServiceCatalog
                {
                    Id = 8,
                    ServiceName = "Балансировка роторов и валов",
                    Price = 3200m,
                    Category = "Производственные услуги",
                    Description = "Динамическая и статическая балансировка вращающихся частей оборудования для снижения вибрации и увеличения срока службы.",
                    ImageUrl = "/images/service_balancing.jpg",
                    CreatedAt = DateTime.Parse("2023-08-10T16:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    UpdatedAt = null
                },

                // Электрооборудование и автоматизация
                new ServiceCatalog
                {
                    Id = 9,
                    ServiceName = "Ремонт промышленного контроллера Siemens S7",
                    Price = 6000m,
                    Category = "Электрооборудование",
                    Description = "Диагностика, ремонт электронных компонентов и программного обеспечения контроллеров Siemens S7.",
                    ImageUrl = "/images/service_siemens_plc.jpg",
                    CreatedAt = DateTime.Parse("2023-09-01T09:30:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    UpdatedAt = null
                },
                new ServiceCatalog
                {
                    Id = 10,
                    ServiceName = "Калибровка датчиков давления",
                    Price = 900m,
                    Category = "Электрооборудование",
                    Description = "Выверка и настройка датчиков давления в соответствии с эталонными стандартами.",
                    ImageUrl = "/images/service_pressure_sensor.jpg",
                    CreatedAt = DateTime.Parse("2023-10-15T10:30:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    UpdatedAt = null
                },
                new ServiceCatalog
                {
                    Id = 11,
                    ServiceName = "Обслуживание систем автоматизации",
                    Price = 4000m,
                    Category = "Электрооборудование",
                    Description = "Комплексное обслуживание систем автоматизации, включая проверку проводки, настройку датчиков и обновление ПО.",
                    ImageUrl = "/images/service_automation.jpg",
                    CreatedAt = DateTime.Parse("2023-11-01T11:30:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    UpdatedAt = null
                },

                // Запчасти и материалы
                new ServiceCatalog
                {
                    Id = 12,
                    ServiceName = "Подшипник скольжения 6205 ZZ",
                    Price = 150m,
                    Category = "Запчасти и материалы",
                    Description = "Высококачественный шарикоподшипник с двухсторонним металлическим уплотнением для различных механизмов.",
                    ImageUrl = "/images/part_bearing.jpg",
                    CreatedAt = DateTime.Parse("2023-01-01T08:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    UpdatedAt = null
                },
                new ServiceCatalog
                {
                    Id = 13,
                    ServiceName = "Уплотнительная манжета 80x100x10",
                    Price = 80m,
                    Category = "Запчасти и материалы",
                    Description = "Высокопрочная гидравлическая манжета для герметизации соединений в условиях высокого давления.",
                    ImageUrl = "/images/part_seal.jpg",
                    CreatedAt = DateTime.Parse("2023-01-01T08:05:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    UpdatedAt = null
                },
                new ServiceCatalog
                {
                    Id = 14,
                    ServiceName = "Комплект прокладок для двигателя Д-240",
                    Price = 1200m,
                    Category = "Запчасти и материалы",
                    Description = "Полный набор высококачественных прокладок, необходимый для капитального ремонта дизельного двигателя Д-240.",
                    ImageUrl = "/images/part_gasket_set.jpg",
                    CreatedAt = DateTime.Parse("2023-01-01T08:10:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    UpdatedAt = null
                },
                new ServiceCatalog
                {
                    Id = 15,
                    ServiceName = "Кабель силовой ВВГнг-LS 3х2.5",
                    Price = 250m,
                    Category = "Запчасти и материалы",
                    Description = "Медный силовой кабель с пониженным дымо- и газовыделением, огнестойкий, для прокладки внутри помещений.",
                    ImageUrl = "/images/part_cable.jpg",
                    CreatedAt = DateTime.Parse("2023-01-01T08:15:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    UpdatedAt = null
                },

                new ServiceCatalog
                {
                    Id = 16,
                    ServiceName = "Токарный станок 1К62 (б/у)",
                    Price = 85000m,
                    Category = "Промышленное оборудование",
                    Description = "Восстановленный универсальный токарно-винторезный станок 1К62, полностью готов к эксплуатации, после капитального ремонта.",
                    ImageUrl = "/images/equipment_lathe.jpg",
                    CreatedAt = DateTime.Parse("2023-01-01T09:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    UpdatedAt = DateTime.Parse("2023-03-20T10:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal) // Пример обновления
                },
                new ServiceCatalog
                {
                    Id = 17,
                    ServiceName = "Сварочный аппарат КЕМППИ MasterTig",
                    Price = 32000m,
                    Category = "Промышленное оборудование",
                    Description = "Профессиональный сварочный аппарат для аргонодуговой сварки (TIG) и ручной дуговой сварки (MMA), с полным комплектом оснастки.",
                    ImageUrl = "/images/equipment_welder.jpg",
                    CreatedAt = DateTime.Parse("2023-02-01T09:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    UpdatedAt = null
                }
            );

            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    OrderNumber = "ORD01",
                    CreatedDate = DateTime.Parse("2024-05-02T10:30:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    ClientId = "client_metalprofile_id", 
                    ManagerId = "manager_john_doe_id",    
                    MasterId = "master_alex_smith_id",    
                    StatusId = 1, 
                    ProblemDescription = "Дизельный двигатель Д-240: Снижение мощности, повышенный расход топлива",
                    StartDate = DateTime.Parse("2024-05-03T09:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    CompletionDate = null, 
                    Cost = 15000m,
                    ManagerComments = "Необходим полный капитальный ремонт, согласовано с клиентом.",
                    MasterComments = null,
                    ClientDisplayName = "ООО 'МеталлПрофиль'"
                },
                new Order
                {
                    Id = 2,
                    OrderNumber = "ORD02",
                    CreatedDate = DateTime.Parse("2024-05-11T12:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    ClientId = "client_hydropress_id",
                    ManagerId = "manager_jane_doe_id",
                    MasterId = "master_bob_johnson_id",
                    StatusId = 2, 
                    ProblemDescription = "Гидравлический пресс П3236: Неисправность гидравлической системы, утечка масла.",
                    StartDate = DateTime.Parse("2024-05-12T10:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    CompletionDate = null,
                    Cost = 1200m,
                    ManagerComments = "Диагностика показала необходимость замены уплотнений.",
                    MasterComments = "Начаты работы по замене гидравлических уплотнений.",
                    ClientDisplayName = "ЧУП 'ГидроПрессСервис'"
                },
                new Order
                {
                    Id = 3,
                    OrderNumber = "ORD03",
                    CreatedDate = DateTime.Parse("2024-05-16T09:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    ClientId = "client_stroymash_id",
                    ManagerId = "manager_john_doe_id",
                    MasterId = "master_alex_smith_id",
                    StatusId = 3, 
                    ProblemDescription = "Токарный станок 1К62: Требуется заточка резцов.",
                    StartDate = DateTime.Parse("2024-05-16T11:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    CompletionDate = DateTime.Parse("2024-05-16T15:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    Cost = 300m,
                    ManagerComments = "Заточка выполнена, инструмент готов к выдаче.",
                    MasterComments = "Заточка произведена по ГОСТ.",
                    ClientDisplayName = "ОАО 'СтройМаш'"
                },
                new Order
                {
                    Id = 4,
                    OrderNumber = "ORD04",
                    CreatedDate = DateTime.Parse("2024-06-06T09:15:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    ClientId = "client_promkomplekt_id",
                    ManagerId = "manager_jane_doe_id",
                    MasterId = "master_bob_johnson_id",
                    StatusId = 1, 
                    ProblemDescription = "Подъемный кран РДК-25: Износ редуктора, повышенный шум при работе.",
                    StartDate = null,
                    CompletionDate = null,
                    Cost = 8000m,
                    ManagerComments = "Ожидаем поступление запчастей для редуктора.",
                    MasterComments = null,
                    ClientDisplayName = "УП 'ПромКомплект'"
                },
                new Order
                {
                    Id = 5,
                    OrderNumber = "ORD05",
                    CreatedDate = DateTime.Parse("2024-06-21T11:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    ClientId = "client_agrotech_id",
                    ManagerId = "manager_john_doe_id",
                    MasterId = "master_alex_smith_id",
                    StatusId = 2, 
                    ProblemDescription = "Насос центробежный НЦ-500: Неисправность, требуется дефектовка для определения причины.",
                    StartDate = DateTime.Parse("2024-06-22T09:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    CompletionDate = null,
                    Cost = 700m,
                    ManagerComments = null,
                    MasterComments = "Начата дефектовка подшипникового узла.",
                    ClientDisplayName = "КФХ 'АгроТех'"
                },
                new Order
                {
                    Id = 6,
                    OrderNumber = "ORD06",
                    CreatedDate = DateTime.Parse("2024-06-23T10:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    ClientId = "client_ivanov_id",
                    ManagerId = "manager_john_doe_id",
                    MasterId = "master_bob_johnson_id",
                    StatusId = 1, 
                    ProblemDescription = "Металлоконструкция (Рама станка): Обнаружена трещина в несущей раме.",
                    StartDate = null,
                    CompletionDate = null,
                    Cost = 2500m,
                    ManagerComments = "Требуются сварочные работы и усиление рамы.",
                    MasterComments = null,
                    ClientDisplayName = "ИП Иванов И.И."
                },
                new Order
                {
                    Id = 7,
                    OrderNumber = "ORD07",
                    CreatedDate = DateTime.Parse("2024-06-19T14:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    ClientId = "client_electrosys_id",
                    ManagerId = "manager_jane_doe_id",
                    MasterId = "master_alex_smith_id",
                    StatusId = 2, 
                    ProblemDescription = "Промышленный контроллер Siemens S7-1200: Сбой в работе, ошибки программы.",
                    StartDate = DateTime.Parse("2024-06-20T09:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    CompletionDate = null,
                    Cost = 6000m,
                    ManagerComments = "Перепрошивка и тестирование контроллера.",
                    MasterComments = "Выявлены программные ошибки, выполняется отладка.",
                    ClientDisplayName = "ОДО 'ЭлектроСистемы'"
                },
                new Order
                {
                    Id = 8,
                    OrderNumber = "ORD08",
                    CreatedDate = DateTime.Parse("2024-06-12T11:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    ClientId = "client_automatika_id",
                    ManagerId = "manager_john_doe_id",
                    MasterId = "master_bob_johnson_id",
                    StatusId = 3, 
                    ProblemDescription = "Датчик давления Rosemount 3051: Показания неточные, требуется калибровка.",
                    StartDate = DateTime.Parse("2024-06-13T09:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    CompletionDate = DateTime.Parse("2024-06-13T16:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    Cost = 900m,
                    ManagerComments = "Калибровка выполнена, датчик соответствует эталонным показаниям.",
                    MasterComments = "Проведена калибровка по всем точкам, отчет сформирован.",
                    ClientDisplayName = "ЗАО 'Автоматика'"
                },
                new Order
                {
                    Id = 9,
                    OrderNumber = "ORD09",
                    CreatedDate = DateTime.Parse("2024-04-12T16:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    ClientId = "client_remstroymash_id",
                    ManagerId = "manager_jane_doe_id",
                    MasterId = null, 
                    StatusId = 4, 
                    ProblemDescription = "Покупка восстановленного токарного станка 1К62.",
                    StartDate = null,
                    CompletionDate = DateTime.Parse("2024-04-15T10:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal), // Дата продажи/отгрузки
                    Cost = 85000m,
                    ManagerComments = "Договор купли-продажи подписан, оплата получена. Готов к отгрузке.",
                    MasterComments = null,
                    ClientDisplayName = "СП 'РемСтройМаш'"
                },
                new Order
                {
                    Id = 10,
                    OrderNumber = "ORD10",
                    CreatedDate = DateTime.Parse("2024-05-04T10:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    ClientId = "client_gomelcasting_id",
                    ManagerId = "manager_john_doe_id",
                    MasterId = null, 
                    StatusId = 1, 
                    ProblemDescription = "Покупка сварочного аппарата КЕМППИ MasterTig 2350.",
                    StartDate = null,
                    CompletionDate = null,
                    Cost = 32000m,
                    ManagerComments = "Оформляются документы на продажу, ждем предоплату.",
                    MasterComments = null,
                    ClientDisplayName = "Гомельский завод литья"
                }
            );

            modelBuilder.Entity<OrderService>().HasData(
                new OrderService { OrderId = 1, ServiceId = 1, Quantity = 1, UnitPrice = 15000m }, 
                new OrderService { OrderId = 1, ServiceId = 14, Quantity = 1, UnitPrice = 1200m }, 
                new OrderService { OrderId = 2, ServiceId = 2, Quantity = 1, UnitPrice = 1200m },  
                new OrderService { OrderId = 3, ServiceId = 3, Quantity = 5, UnitPrice = 300m },   
                new OrderService { OrderId = 4, ServiceId = 4, Quantity = 1, UnitPrice = 8000m },  
                new OrderService { OrderId = 5, ServiceId = 5, Quantity = 1, UnitPrice = 700m },   
                new OrderService { OrderId = 6, ServiceId = 6, Quantity = 1, UnitPrice = 2500m }, 
                new OrderService { OrderId = 7, ServiceId = 9, Quantity = 1, UnitPrice = 6000m },  
                new OrderService { OrderId = 8, ServiceId = 10, Quantity = 1, UnitPrice = 900m }, 
                new OrderService { OrderId = 9, ServiceId = 16, Quantity = 1, UnitPrice = 85000m },
                new OrderService { OrderId = 10, ServiceId = 17, Quantity = 1, UnitPrice = 32000m } 
            );

            modelBuilder.Entity<OrderLog>().HasData(
                new OrderLog
                {
                    Id = 1,
                    EventDate = DateTime.Parse("2024-05-02T10:35:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    OrderId = 1,
                    OrderNumber = "ORD01",
                    ItemCode = "SRV001",
                    ItemName = "Капитальный ремонт дизельного двигателя Д-240",
                    CustomerName = "ООО 'МеталлПрофиль'",
                    EventDescription = "Заказ создан. Зарегистрирована заявка на ремонт двигателя.",
                    UserId = "manager_john_doe_id",
                    UserDisplayName = "Джон Доу (Менеджер)"
                },
                new OrderLog
                {
                    Id = 2,
                    EventDate = DateTime.Parse("2024-05-03T09:10:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    OrderId = 1,
                    OrderNumber = "ORD01",
                    ItemCode = "SRV001",
                    ItemName = "Начало работ по ремонту Д-240",
                    CustomerName = "ООО 'МеталлПрофиль'",
                    EventDescription = "Начаты работы по капитальному ремонту двигателя. Передан мастеру.",
                    UserId = "master_alex_smith_id",
                    UserDisplayName = "Алекс Смит (Мастер)"
                },
                new OrderLog
                {
                    Id = 3,
                    EventDate = DateTime.Parse("2024-05-11T12:15:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    OrderId = 2,
                    OrderNumber = "ORD02",
                    ItemCode = "SRV002",
                    ItemName = "Диагностика гидравлической системы пресса",
                    CustomerName = "ЧУП 'ГидроПрессСервис'",
                    EventDescription = "Заказ создан. Зарегистрирована заявка на диагностику гидравлики.",
                    UserId = "manager_jane_doe_id",
                    UserDisplayName = "Джейн Доу (Менеджер)"
                },
                new OrderLog
                {
                    Id = 4,
                    EventDate = DateTime.Parse("2024-05-12T10:30:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    OrderId = 2,
                    OrderNumber = "ORD02",
                    ItemCode = "SRV002",
                    ItemName = "Дефектовка гидравлической системы",
                    CustomerName = "ЧУП 'ГидроПрессСервис'",
                    EventDescription = "Проведена дефектовка, выявлена необходимость замены уплотнений.",
                    UserId = "master_bob_johnson_id",
                    UserDisplayName = "Боб Джонсон (Мастер)"
                },
                new OrderLog
                {
                    Id = 5,
                    EventDate = DateTime.Parse("2024-05-16T15:05:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    OrderId = 3,
                    OrderNumber = "ORD03",
                    ItemCode = "SRV003",
                    ItemName = "Заточка инструмента для металлообработки",
                    CustomerName = "ОАО 'СтройМаш'",
                    EventDescription = "Работы по заточке завершены. Заказ готов к выдаче.",
                    UserId = "master_alex_smith_id",
                    UserDisplayName = "Алекс Смит (Мастер)"
                },
              
                new OrderLog
                {
                    Id = 6,
                    EventDate = DateTime.Parse("2024-06-06T09:30:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    OrderId = 4,
                    OrderNumber = "ORD04",
                    ItemCode = "SRV004",
                    ItemName = "Ремонт редуктора крана РДК-25",
                    CustomerName = "УП 'ПромКомплект'",
                    EventDescription = "Заказ создан. Ожидание поставки запчастей для редуктора.",
                    UserId = "manager_jane_doe_id",
                    UserDisplayName = "Джейн Доу (Менеджер)"
                },
                new OrderLog
                {
                    Id = 7,
                    EventDate = DateTime.Parse("2024-06-15T10:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    OrderId = 4,
                    OrderNumber = "ORD04",
                    ItemCode = "PART01", 
                    ItemName = "Подшипники для редуктора",
                    CustomerName = "УП 'ПромКомплект'",
                    EventDescription = "Запчасти для редуктора получены на склад.",
                    UserId = "manager_jane_doe_id",
                    UserDisplayName = "Джейн Доу (Менеджер)"
                },
                new OrderLog
                {
                    Id = 8,
                    EventDate = DateTime.Parse("2024-06-22T09:30:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    OrderId = 5,
                    OrderNumber = "ORD05",
                    ItemCode = "SRV005",
                    ItemName = "Дефектовка насосного оборудования",
                    CustomerName = "КФХ 'АгроТех'",
                    EventDescription = "Дефектовка узлов и агрегатов насоса завершена. Согласование стоимости ремонта с клиентом.",
                    UserId = "master_alex_smith_id",
                    UserDisplayName = "Алекс Смит (Мастер)"
                },
                new OrderLog
                {
                    Id = 9,
                    EventDate = DateTime.Parse("2024-06-23T10:10:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    OrderId = 6,
                    OrderNumber = "ORD06",
                    ItemCode = "SRV006",
                    ItemName = "Сварочные работы по восстановлению конструкции",
                    CustomerName = "ИП Иванов И.И.",
                    EventDescription = "Заказ создан. Требуется оценка масштаба сварочных работ.",
                    UserId = "manager_john_doe_id",
                    UserDisplayName = "Джон Доу (Менеджер)"
                },
                new OrderLog
                {
                    Id = 10,
                    EventDate = DateTime.Parse("2024-06-21T11:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    OrderId = 7,
                    OrderNumber = "ORD07",
                    ItemCode = "SRV009",
                    ItemName = "Ремонт промышленного контроллера Siemens S7",
                    CustomerName = "ОДО 'ЭлектроСистемы'",
                    EventDescription = "Программный ремонт контроллера Siemens S7 завершен, проведено тестирование.",
                    UserId = "master_alex_smith_id",
                    UserDisplayName = "Алекс Смит (Мастер)"
                },
                new OrderLog
                {
                    Id = 11,
                    EventDate = DateTime.Parse("2024-06-13T16:15:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    OrderId = 8,
                    OrderNumber = "ORD08",
                    ItemCode = "SRV010",
                    ItemName = "Калибровка датчиков давления",
                    CustomerName = "ЗАО 'Автоматика'",
                    EventDescription = "Калибровка датчика успешно завершена. Оформлены протоколы калибровки.",
                    UserId = "master_bob_johnson_id",
                    UserDisplayName = "Боб Джонсон (Мастер)"
                },
                new OrderLog
                {
                    Id = 12,
                    EventDate = DateTime.Parse("2024-04-12T16:30:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    OrderId = 9,
                    OrderNumber = "ORD09",
                    ItemCode = "EQP001", 
                    ItemName = "Токарный станок 1К62 (б/у)",
                    CustomerName = "СП 'РемСтройМаш'",
                    EventDescription = "Заказ продажи станка создан. Согласование условий.",
                    UserId = "manager_jane_doe_id",
                    UserDisplayName = "Джейн Доу (Менеджер)"
                },
                new OrderLog
                {
                    Id = 13,
                    EventDate = DateTime.Parse("2024-04-15T10:30:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    OrderId = 9,
                    OrderNumber = "ORD09",
                    ItemCode = "EQP001",
                    ItemName = "Отгрузка токарного станка 1К62",
                    CustomerName = "СП 'РемСтройМаш'",
                    EventDescription = "Токарный станок 1К62 отгружен клиенту. Оплата получена в полном объеме.",
                    UserId = "manager_jane_doe_id",
                    UserDisplayName = "Джейн Доу (Менеджер)"
                },
                new OrderLog
                {
                    Id = 14,
                    EventDate = DateTime.Parse("2024-05-04T10:15:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                    OrderId = 10,
                    OrderNumber = "ORD10",
                    ItemCode = "EQP002",
                    ItemName = "Сварочный аппарат КЕМППИ MasterTig",
                    CustomerName = "Гомельский завод литья",
                    EventDescription = "Заказ продажи сварочного аппарата создан. Выставлен счет на предоплату.",
                    UserId = "manager_john_doe_id",
                    UserDisplayName = "Джон Доу (Менеджер)"
                }
            );
        }
    }
}
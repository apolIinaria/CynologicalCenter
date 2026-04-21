using CynologicalCenter.Data;
using CynologicalCenter.Data.Repositories;
using CynologicalCenter.Data.StoredProcedures;
using CynologicalCenter.Services;
using CynologicalCenter.UI;
using CynologicalCenter.UI.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CynologicalCenter
{
    public partial class App : Application
    {
        public static DbConnectionFactory DbFactory { get; private set; } = null!;
        public static ProcedureCaller Procedures { get; private set; } = null!;
        public static BreedRepository Breeds { get; private set; } = null!;
        public static OwnerRepository Owners { get; private set; } = null!;
        public static TrainerRepository Trainers { get; private set; } = null!;
        public static CourseRepository Courses { get; private set; } = null!;
        public static DogRepository Dogs { get; private set; } = null!;
        public static SessionRepository Sessions { get; private set; } = null!;
        public static AuditRepository Audit { get; private set; } = null!;
        public static EnrollmentService EnrollmentService { get; private set; } = null!;
        public static SessionService SessionService { get; private set; } = null!;
        public static OwnerService OwnerService { get; private set; } = null!;
        public static TrainerService TrainerService { get; private set; } = null!;
        public static ReportService ReportService { get; private set; } = null!;
        public static SecurityService SecurityService { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DispatcherUnhandledException += (s, ex) =>
            {
                MessageBox.Show(
                    $"Непередбачена помилка:\n{ex.Exception.Message}",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                ex.Handled = true;
            };

            string photosRoot = ConfigurationManager.AppSettings["PhotosRootPath"]!;
            Directory.CreateDirectory(Path.Combine(photosRoot, "dogs"));
            Directory.CreateDirectory(Path.Combine(photosRoot, "trainers"));
            Directory.CreateDirectory(
                ConfigurationManager.AppSettings["LogsPath"]!);
            DbFactory = new DbConnectionFactory();
            Procedures = new ProcedureCaller(DbFactory);
            Breeds = new BreedRepository(DbFactory);
            Owners = new OwnerRepository(DbFactory);
            Trainers = new TrainerRepository(DbFactory);
            Courses = new CourseRepository(DbFactory);
            Dogs = new DogRepository(DbFactory);
            Sessions = new SessionRepository(DbFactory);
            Audit = new AuditRepository(DbFactory);
            EnrollmentService = new EnrollmentService(Procedures, Dogs);
            SessionService = new SessionService(Procedures, Sessions);
            OwnerService = new OwnerService(Owners, Procedures);
            TrainerService = new TrainerService(Trainers);
            ReportService = new ReportService(Procedures);
            SecurityService = new SecurityService(DbFactory);

            var login = new LoginWindow();
            login.Show();
        }
    }
}

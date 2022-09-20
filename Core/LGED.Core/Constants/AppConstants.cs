using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LGED.Core.Constants
{
    public class AppConstants
    {
        public static class Role
        {
            public const string MasterAdministrator = "Master Admin";
            public const string Admin = "Admin";
            public const string Normal = "Normal";
        }

        public static List<string> AdminRoles = new List<string> { Role.MasterAdministrator };

        /// <summary>
        /// Used to declare custom header for http request/response
        /// </summary>
        public static class HttpHeader
        {
            public const string LGED_TENANT_HEADER = "x-lged-tenant";
            public const string LGED_AREA_HEADER = "x-lged-area";
            public const string LGED_COMPANY_ID_HEADER = "x-lged-company-id";
        }

        public static class ComponentStatus
        {
            public const string Draft = "Draft";
            public const string Submitted = "Submitted";
        }

        public static class ComponentInputType
        {
            public const string ManualInput = "Manual Data";
            public const string SkinTem = "Skin Temperature";
            public const string TmtTem = "Calculated TMT";
            public const string ThermographyData = "Thermography Data";
        }

        public static class DateFormat
        {
            public const string DEFAULT_DATE_FORMAT = "dd/MM/yyyy";
            public const string DEFAULT_DATETIME_FORMAT = "dd/MM/yyyy H:mm tt";
            public const string DEFAULT_DATETIME24_FORMAT = "dd/MM/yyyy HH:mm";
            public const string DEFAULT_TIME_FORMAT = "HH:mm:ss";
        }

        public static class TimeZoneFormat
        {
            public const string DEFAULT_TIME_ZONE = "Singapore Standard Time";
            public static TimeZoneInfo SingaporeTimeZone = TimeZoneInfo.FindSystemTimeZoneById(DEFAULT_TIME_ZONE);
        }

        /// <summary>
        /// Use with ApiWrapperMiddleware to exclude paths have not been wrapped
        /// </summary>
        public static class PluginUrl
        {
            public const string Swagger = "/swagger";
            public const string HangFire = "/hangfire";
            public const string Manager = "/manager";
            public const string Attachment = "/api/attachment";
            public const string Hub = "/api/hub";
        }

        /// <summary>
        /// Oxide Acid
        /// </summary>
        public static class MasterDataType
        {
            //lged
            public const string Objectives = "Objectives";
            public const string FacilitiesStatus = "FacilitiesStatus";
            public const string PetronasTechnicalStandard = "PetronasTechnicalStandard";
            public const string PetronasGuideline = "PetronasGuideline";
            public const string InternationalCodeAndStandard = "InternationalCodeAndStandard";
            public const string ReferenceDocument = "ReferenceDocument";
            public const string FacilityHistory = "FacilityHistory";
            public const string Abbreviation = "Abbreviation";
            public const string ProcessDataStatus = "ProcessDataStatus";
            public const string Term = "Term";
            public const string FlowPattern = "FlowPattern";
            public const string CorrosionType = "CorrosionType";

            //Equipment
            public const string Equipment = "Equipment";
            public const string EquipmentType = "Equipment Type";
            public const string Component = "Component";
            public const string DesignCode = "Design Code";

            // Component
            public const string TemperatureInput = "Temperature Input";

            public const string PressureInput = "Pressure Input";
            public const string ComponentShape = "Component Shape";
            public const string InitialProductForm = "Initial Product Form";

            public const string TubeArrangement = "Tube Arrangement";
            public const string FlowType = "Flow Type";
            public const string CorrosionRate = "Corrosion Rate";
            public const string FluidService = "Fluid Service";

            // Pi
            public const string PiType = "PI Tag Type";

            public const string Pressure = "Pressure";
            public const string Temperature = "Temperature";
            public const string FlowRate = "Flow Rate";
            public const string AssetMasterData = "Asset Master Data";
        }

        public static class EmailTemplate
        {
            public const string RequestAccessRejected = "Templates/Email/RequestAccess_Rejected.html";
            public const string RequestAccessApprovedInternal = "Templates/Email/RequestAccess_Approved_Internal.html";
            public const string RequestAccessApprovedExternal = "Templates/Email/RequestAccess_Approved_External.html";
            public const string RequestAccessSubmittedAdmin = "Templates/Email/RequestAccess_Submitted_Admin.html";
            public const string RequestAccessSubmittedUser = "Templates/Email/RequestAccess_Submitted_User.html";

            // Data Anomalies
            public const string DataAnomaliesIowWarning = "Templates/Email/IowNotification.cshtml";
            public const string DataAnomaliesAlert = "Templates/Email/DataAnomalies_Alert.html";
            public const string RemainingLifeCreepAlert = "Templates/Email/RemainingLife_CreepAlert.html";
            public const string RemainingLifeThinningAlert = "Templates/Email/RemainingLife_ThinningAlert.html";

        }
        public static class ExcelTemplate
        {
            public const string PiTagMedian = "Templates/Excel/PiTagMedianTemplate.xlsx";


        }

        public static class TemplateStatus
        {
            public const string WaitForValidation = "Queue for processing";

            public const string TemplateInvalid = "Template Invalid";
            public const string TemplateValidated = "Template Valid";


            public const string DataValid = "Data Valid";
            public const string DataInvalid = "Data Invalid";

            public const string DataUpdated = "Successfully  Updated";
            public const string DataUpdateFailed = "Unsuccessfully Update";
        }

        public static class UploadTemplateType
        {

            public const string PiTagMedian = "Pi Tag Median";
        }
    }
}
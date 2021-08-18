using Ext_Dynamics_API.Canvas.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class Course
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("sis_course_id")]
        public string SisCourseId { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("integration_id")]
        public string IntegrationId { get; set; }

        [JsonProperty("sis_import_id")]
        public int? SisImportId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("course_code")]
        public string CourseCode { get; set; }

        [JsonProperty("workflow_state")]
        public CourseState WorkflowState { get; set; }

        [JsonProperty("account_id")]
        public int? AccountId { get; set; }

        [JsonProperty("root_account_id")]
        public int? RootAccountId { get; set; }

        [JsonProperty("enrollment_term_id")]
        public int? EnrollmentTermId { get; set; }

        [JsonProperty("grading_periods")]
        public List<GradingPeriod> GradingPeriods { get; set; }

        [JsonProperty("grading_standard_id")]
        public int? GradingStandardId { get; set; }

        [JsonProperty("grade_passback_setting")]
        public string GradePassbackSetting { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("start_at")]
        public DateTime? StartAt { get; set; }

        [JsonProperty("end_at")]
        public DateTime? EndAt { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("enrollments")]
        public List<Enrollment> Enrollments { get; set; }

        [JsonProperty("total_students")]
        public int? TotalStudents { get; set; }

        [JsonProperty("calendar")]
        public object Calendar { get; set; }

        [JsonProperty("default_view")]
        public string DefaultView { get; set; }

        [JsonProperty("syllabus_body")]
        public string SyllabusBody { get; set; }

        [JsonProperty("needs_grading_count")]
        public int? NeedsGradingCount { get; set; }

        [JsonProperty("term")]
        public Term Term { get; set; }

        [JsonProperty("course_progress")]
        public CourseProgress CourseProgress { get; set; }

        [JsonProperty("apply_assignment_group_weights")]
        public bool? ApplyAssignmentGroupWeights { get; set; }

        [JsonProperty("permissions")]
        public Permissions Permissions { get; set; }

        [JsonProperty("is_public")]
        public bool? IsPublic { get; set; }

        [JsonProperty("is_public_to_auth_users")]
        public bool? IsPublicToAuthUsers { get; set; }

        [JsonProperty("public_syllabus")]
        public bool? PublicSyllabus { get; set; }

        [JsonProperty("public_syllabus_to_auth")]
        public bool? PublicSyllabusToAuth { get; set; }

        [JsonProperty("public_description")]
        public string PublicDescription { get; set; }

        [JsonProperty("storage_quota_mb")]
        public int? StorageQuotaMb { get; set; }

        [JsonProperty("storage_quota_used_mb")]
        public int? StorageQuotaUsedMb { get; set; }

        [JsonProperty("hide_final_grades")]
        public bool? HideFinalGrades { get; set; }

        [JsonProperty("license")]
        public string License { get; set; }

        [JsonProperty("allow_student_assignment_edits")]
        public bool? AllowStudentAssignmentEdits { get; set; }

        [JsonProperty("allow_wiki_comments")]
        public bool? AllowWikiComments { get; set; }

        [JsonProperty("allow_student_forum_attachments")]
        public bool? AllowStudentForumAttachments { get; set; }

        [JsonProperty("open_enrollment")]
        public bool? OpenEnrollment { get; set; }

        [JsonProperty("self_enrollment")]
        public bool? SelfEnrollment { get; set; }

        [JsonProperty("restrict_enrollments_to_course_dates")]
        public bool? RestrictEnrollmentsToCourseDates { get; set; }

        [JsonProperty("course_format")]
        public string CourseFormat { get; set; }

        [JsonProperty("access_restricted_by_date")]
        public bool? AccessRestrictedByDate { get; set; }

        [JsonProperty("time_zone")]
        public string TimeZone { get; set; }

        [JsonProperty("blueprint")]
        public bool? Blueprint { get; set; }

        [JsonProperty("blueprint_restrictions")]
        public BlueprintRestrictions BlueprintRestrictions { get; set; }

        [JsonProperty("blueprint_restrictions_by_object_type")]
        public BlueprintRestrictionsByObjectType BlueprintRestrictionsByObjectType { get; set; }

        [JsonProperty("template")]
        public bool? Template { get; set; }
    }
}

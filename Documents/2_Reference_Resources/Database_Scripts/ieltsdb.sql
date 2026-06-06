-- WebIeltsFree Master SQL Setup
-- Optimized for phpMyAdmin / MariaDB / MySQL
--
CREATE DATABASE IF NOT EXISTS `ieltsdb`;
USE `ieltsdb`;

SET FOREIGN_KEY_CHECKS = 0;
SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `ieltsdb`
--

-- --------------------------------------------------------

--
-- Table structure for table `tb_admin_actions`
--

CREATE TABLE `tb_admin_actions` (
  `action_id` int(11) NOT NULL,
  `admin_id` int(11) DEFAULT NULL,
  `action_type` varchar(255) DEFAULT NULL,
  `action_time` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_admin_actions`
--

INSERT INTO `tb_admin_actions` (`action_id`, `admin_id`, `action_type`, `action_time`) VALUES
(1, 2, 'publish_course', '2026-04-12 04:33:21');

-- --------------------------------------------------------

--
-- Table structure for table `tb_ai_roadmaps`
--

CREATE TABLE `tb_ai_roadmaps` (
  `roadmap_id` int(11) NOT NULL,
  `user_id` int(11) DEFAULT NULL,
  `generated_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `estimated_weeks` int(11) DEFAULT NULL,
  `target_band` float DEFAULT NULL COMMENT 'Target IELTS band score (0-9)'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_ai_roadmaps`
--

INSERT INTO `tb_ai_roadmaps` (`roadmap_id`, `user_id`, `generated_at`, `estimated_weeks`, `target_band`) VALUES
(1, 1, '2026-04-12 04:33:21', NULL, NULL),
(2, 3, '2026-05-16 02:49:15', NULL, NULL),
(3, 3, '2026-05-16 02:49:18', NULL, NULL),
(4, 8, '2026-05-16 02:51:14', NULL, NULL),
(5, 8, '2026-05-16 02:53:08', NULL, NULL),
(6, 3, '2026-05-16 02:57:48', NULL, NULL),
(7, 3, '2026-05-16 02:57:50', NULL, NULL),
(8, 3, '2026-05-16 02:57:52', NULL, NULL),
(9, 9, '2026-05-16 07:24:44', NULL, NULL),
(10, 9, '2026-05-16 07:24:45', NULL, NULL),
(11, 9, '2026-05-16 07:25:12', NULL, NULL),
(12, 9, '2026-05-16 07:31:07', NULL, NULL),
(13, 9, '2026-05-16 07:31:28', 52, 7),
(14, 9, '2026-05-16 07:31:46', NULL, NULL),
(15, 9, '2026-05-16 07:31:47', NULL, NULL),
(16, 9, '2026-05-16 07:44:21', NULL, NULL),
(17, 9, '2026-05-16 08:09:19', 52, 7),
(18, 9, '2026-05-16 08:09:29', NULL, NULL),
(19, 9, '2026-05-16 08:09:30', NULL, NULL),
(20, 9, '2026-05-16 08:14:07', NULL, NULL),
(21, 3, '2026-05-18 02:12:47', NULL, NULL),
(22, 3, '2026-05-18 02:13:42', NULL, NULL),
(23, 3, '2026-05-18 02:23:14', NULL, NULL),
(24, 3, '2026-05-18 02:24:25', NULL, NULL),
(25, 7, '2026-05-18 06:54:46', NULL, NULL),
(26, 3, '2026-05-18 08:57:03', NULL, NULL),
(27, 3, '2026-05-19 01:55:36', NULL, NULL),
(28, 3, '2026-05-19 07:25:39', NULL, NULL),
(29, 3, '2026-05-19 19:59:30', NULL, NULL),
(30, 3, '2026-05-19 20:10:03', NULL, NULL),
(31, 3, '2026-05-20 02:53:18', NULL, NULL),
(32, 3, '2026-05-20 08:18:04', NULL, NULL),
(33, 3, '2026-05-20 17:52:27', NULL, NULL),
(34, 3, '2026-05-20 18:09:02', NULL, NULL),
(35, 3, '2026-05-20 19:31:57', 56, 7.5),
(36, 3, '2026-05-20 19:32:01', NULL, NULL),
(37, 3, '2026-05-20 19:32:03', NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `tb_ai_roadmap_steps`
--

CREATE TABLE `tb_ai_roadmap_steps` (
  `step_id` int(11) NOT NULL,
  `roadmap_id` int(11) DEFAULT NULL,
  `lesson_id` int(11) DEFAULT NULL,
  `week_number` int(11) DEFAULT NULL,
  `priority` int(11) DEFAULT 0,
  `is_completed` tinyint(1) DEFAULT 0,
  `completed_at` timestamp NULL DEFAULT NULL,
  `step_type` varchar(50) DEFAULT NULL,
  `description` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_ai_roadmap_steps`
--

INSERT INTO `tb_ai_roadmap_steps` (`step_id`, `roadmap_id`, `lesson_id`, `week_number`, `priority`, `is_completed`, `completed_at`, `step_type`, `description`) VALUES
(1, 1, 1, 1, 1, 0, NULL, 'lesson', 'Complete TFNG Foundations lesson and review mistakes.'),
(2, 3, NULL, NULL, 0, 0, NULL, 'teacher_suggested', 'a'),
(3, 13, 1, 1, 1, 0, NULL, NULL, NULL),
(4, 13, 2, 1, 2, 0, NULL, NULL, NULL),
(5, 13, 4, 2, 3, 0, NULL, NULL, NULL),
(6, 13, 401, 2, 4, 0, NULL, NULL, NULL),
(7, 13, 409, 3, 5, 0, NULL, NULL, NULL),
(8, 13, 423, 3, 6, 0, NULL, NULL, NULL),
(9, 13, 3, 4, 7, 0, NULL, NULL, NULL),
(10, 13, 402, 4, 8, 0, NULL, NULL, NULL),
(11, 13, 403, 5, 9, 0, NULL, NULL, NULL),
(12, 13, 404, 5, 10, 0, NULL, NULL, NULL),
(13, 13, 405, 6, 11, 0, NULL, NULL, NULL),
(14, 13, 410, 6, 12, 0, NULL, NULL, NULL),
(15, 13, 411, 7, 13, 0, NULL, NULL, NULL),
(16, 13, 412, 7, 14, 0, NULL, NULL, NULL),
(17, 13, 415, 8, 15, 0, NULL, NULL, NULL),
(18, 13, 416, 8, 16, 0, NULL, NULL, NULL),
(19, 17, 1, 1, 1, 0, NULL, NULL, NULL),
(20, 17, 2, 1, 2, 0, NULL, NULL, NULL),
(21, 17, 4, 2, 3, 0, NULL, NULL, NULL),
(22, 17, 401, 2, 4, 0, NULL, NULL, NULL),
(23, 17, 409, 3, 5, 0, NULL, NULL, NULL),
(24, 17, 423, 3, 6, 0, NULL, NULL, NULL),
(25, 17, 3, 4, 7, 0, NULL, NULL, NULL),
(26, 17, 402, 4, 8, 0, NULL, NULL, NULL),
(27, 17, 403, 5, 9, 0, NULL, NULL, NULL),
(28, 17, 404, 5, 10, 0, NULL, NULL, NULL),
(29, 17, 405, 6, 11, 0, NULL, NULL, NULL),
(30, 17, 410, 6, 12, 0, NULL, NULL, NULL),
(31, 17, 411, 7, 13, 0, NULL, NULL, NULL),
(32, 17, 412, 7, 14, 0, NULL, NULL, NULL),
(33, 17, 415, 8, 15, 0, NULL, NULL, NULL),
(34, 17, 416, 8, 16, 0, NULL, NULL, NULL),
(35, 35, 1, 1, 1, 0, NULL, NULL, NULL),
(36, 35, 2, 1, 2, 0, NULL, NULL, NULL),
(37, 35, 4, 2, 3, 0, NULL, NULL, NULL),
(38, 35, 401, 2, 4, 0, NULL, NULL, NULL),
(39, 35, 409, 3, 5, 0, NULL, NULL, NULL),
(40, 35, 423, 3, 6, 0, NULL, NULL, NULL),
(41, 35, 2001, 4, 7, 0, NULL, NULL, NULL),
(42, 35, 2002, 4, 8, 0, NULL, NULL, NULL),
(43, 35, 2005, 5, 9, 0, NULL, NULL, NULL),
(44, 35, 2010, 5, 10, 0, NULL, NULL, NULL),
(45, 35, 3, 6, 11, 0, NULL, NULL, NULL),
(46, 35, 402, 6, 12, 0, NULL, NULL, NULL),
(47, 35, 403, 7, 13, 0, NULL, NULL, NULL),
(48, 35, 404, 7, 14, 0, NULL, NULL, NULL),
(49, 35, 405, 8, 15, 0, NULL, NULL, NULL),
(50, 35, 410, 8, 16, 0, NULL, NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `tb_ai_skill_analysis`
--

CREATE TABLE `tb_ai_skill_analysis` (
  `id` int(11) NOT NULL,
  `user_id` int(11) DEFAULT NULL,
  `reading_score` float DEFAULT NULL,
  `listening_score` float DEFAULT NULL,
  `writing_score` float DEFAULT NULL,
  `speaking_score` float DEFAULT NULL,
  `analyzed_at` datetime DEFAULT NULL,
  `strengths` text DEFAULT NULL COMMENT 'AI-identified strengths',
  `weaknesses` text DEFAULT NULL COMMENT 'AI-identified weaknesses',
  `recommendations` text DEFAULT NULL COMMENT 'AI-generated study recommendations'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_ai_skill_analysis`
--

INSERT INTO `tb_ai_skill_analysis` (`id`, `user_id`, `reading_score`, `listening_score`, `writing_score`, `speaking_score`, `analyzed_at`, `strengths`, `weaknesses`, `recommendations`) VALUES
(1, 1, 6, 6.5, 6, 6, NULL, NULL, NULL, NULL),
(2, 9, 0.5, 0.5, 0.5, 0.5, '2026-05-16 14:31:28', 'No dominant skill yet', 'listening, reading, writing, speaking', 'Your current English proficiency is foundational (A0/A1).\n**Prioritize building core English skills before IELTS preparation.**\n1.  Focus on mastering basic vocabulary and grammar (A0-A2 level).\n2.  Practice listening to very simple English conversations daily.\n3.  Read basic texts like graded readers (A1-A2).\n4.  Form simple sentences for speaking and writing practice.\n5.  Seek a beginner English course or tutor for structured learning.\n6.  Consistent daily practice is crucial.\nRe-evaluate IELTS preparation once you reach a solid A2 level.'),
(3, 9, 0.5, 0.5, 0.5, 0.5, '2026-05-16 15:09:19', 'No dominant skill yet', 'listening, reading, writing, speaking', 'Your current English proficiency is significantly below the level required for IELTS.\nRecommendations:\n1.  Focus entirely on building foundational English skills: A1/A2 grammar and essential vocabulary.\n2.  Engage daily with simple English listening materials (e.g., beginner podcasts, graded audio).\n3.  Practice reading very basic English texts (e.g., graded readers, simple articles).\n4.  Familiarize yourself with IELTS task types, but do not attempt full tests yet.\n5.  Concentrate on understanding basic sentences and extracting key information.\n6.  Consider enrolling in a general English course to establish a strong language base.'),
(4, 3, 0.5, 0.5, 0, 0, '2026-05-21 02:31:57', 'No dominant skill yet', 'listening, reading, writing, speaking', 'Your current English level is foundational, far below IELTS entry.\nDo NOT focus on IELTS strategies yet. Your score indicates no basic comprehension or production.\nEnroll in an intensive General English course (A1/A2 level recommended).\nBuild core vocabulary and grammar from scratch.\nPractice listening to simple English daily (e.g., beginner podcasts, short videos).\nRead simplified texts to develop basic comprehension.\nFocus on forming simple sentences for writing and speaking practice.\nRe-take a general English placement test in 3-6 months.');

-- --------------------------------------------------------

--
-- Table structure for table `tb_announcements`
--

CREATE TABLE `tb_announcements` (
  `announcement_id` int(11) NOT NULL,
  `teacher_id` int(11) NOT NULL,
  `title` varchar(255) NOT NULL,
  `content` text NOT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` datetime DEFAULT current_timestamp(),
  `expires_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_announcements`
--

INSERT INTO `tb_announcements` (`announcement_id`, `teacher_id`, `title`, `content`, `is_active`, `created_at`, `expires_at`) VALUES
(2, 7, 'Announcement', 'Hi', 1, '2026-05-16 07:59:44', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `tb_answers`
--

CREATE TABLE `tb_answers` (
  `answer_id` int(11) NOT NULL,
  `question_id` int(11) DEFAULT NULL,
  `correct_answer` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_answers`
--

INSERT INTO `tb_answers` (`answer_id`, `question_id`, `correct_answer`) VALUES
(1, 8088, 'a');

-- --------------------------------------------------------

--
-- Table structure for table `tb_audit_logs`
--

CREATE TABLE `tb_audit_logs` (
  `log_id` bigint(20) NOT NULL,
  `user_id` int(11) DEFAULT NULL,
  `admin_id` int(11) DEFAULT NULL,
  `action` varchar(255) NOT NULL,
  `entity_type` varchar(100) DEFAULT NULL,
  `entity_id` int(11) DEFAULT NULL,
  `old_values_json` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`old_values_json`)),
  `new_values_json` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`new_values_json`)),
  `ip_address` varchar(45) DEFAULT NULL,
  `user_agent` varchar(500) DEFAULT NULL,
  `status` enum('success','failure') DEFAULT 'success',
  `error_message` text DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_audit_logs`
--

INSERT INTO `tb_audit_logs` (`log_id`, `user_id`, `admin_id`, `action`, `entity_type`, `entity_id`, `old_values_json`, `new_values_json`, `ip_address`, `user_agent`, `status`, `error_message`, `created_at`) VALUES
(1, 1, 2, 'UPDATE_PROFILE', 'tb_user_profiles', 1, '{\"full_name\":\"Old Name\"}', '{\"full_name\":\"Nguyen Van A\"}', '127.0.0.1', 'Mozilla/5.0', 'success', NULL, '2026-04-12 04:33:21'),
(5, 6, NULL, 'create_assignment', 'TeacherAssignment', 2, NULL, '{\"AssignmentId\":2,\"TestId\":601,\"TeacherId\":6,\"StudentId\":3,\"Title\":\"Weekly Academic Test\",\"Instructions\":null,\"Deadline\":null,\"Status\":\"pending\",\"AssignedAt\":\"2026-05-16T05:49:43.2669231Z\",\"CompletedAt\":null,\"AttemptId\":null,\"IsDeleted\":false,\"Teacher\":null,\"Student\":null,\"Test\":null}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-15 22:49:43'),
(6, 6, NULL, 'create_teacher_profile', 'TeacherProfile', 6, NULL, '{\"TeacherId\":6,\"Bio\":\"\",\"Specialties\":\"\",\"YearsExperience\":null,\"IsPublic\":true,\"CreatedAt\":\"2026-05-16T05:57:58.5781701Z\",\"UpdatedAt\":null}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-15 22:57:58'),
(7, 6, NULL, 'update_teacher_profile', 'TeacherProfile', 6, '{\"TeacherId\":6,\"Bio\":\"\",\"Specialties\":\"\",\"YearsExperience\":null,\"IsPublic\":true,\"CreatedAt\":\"2026-05-16T05:57:58\",\"UpdatedAt\":null}', '{\"TeacherId\":6,\"Bio\":\"Full Name: Emma L. Clark\\r\\nSpecialties: Writing Task 2, Speaking Strategy, Academic Vocabulary\\r\\nYears of Experience: 6 years\\r\\n\\u0026quot;Hello everyone, I am Emma L. Clark. With over 6 years of specialized experience in teaching and preparing students for the IELTS Academic exam, I have helped more than 1,000 students successfully jump from a 5.0 to a 7.5\\u002B band score.\\r\\n\\r\\nMy teaching philosophy focuses on optimizing linguistic thinking rather than rote memorization of sample essays. At WebIeltsFree, my primary responsibilities include:\\r\\n\\r\\nAuditing and approving new Writing and Speaking practice materials.\\r\\n\\r\\nDirectly grading and resolving disputes regarding AI-generated test results to ensure absolute accuracy.\\r\\n\\r\\nProviding personalized learning roadmaps tailored to each student\\u0026#39;s actual proficiency level.\\r\\n\\r\\nCredentials \\u0026amp; Certifications:\\r\\n\\r\\nIELTS Academic: 8.0 Overall (Listening 8.5, Reading 8.0, Writing 7.5, Speaking 8.0).\\r\\n\\r\\nTESOL International Teaching Certification.\\r\\n\\r\\nBachelor\\u2019s Degree in English Education.\\r\\n\\r\\nI believe that by combining modern AI technology with the practical expertise of our teaching staff, you can conquer your IELTS goals faster and more accurately than ever before.\\u0026quot;\",\"Specialties\":\"I have a 8.0 ielts certificate\",\"YearsExperience\":6,\"IsPublic\":true,\"CreatedAt\":\"2026-05-16T05:57:58\",\"UpdatedAt\":\"2026-05-16T06:03:10.5248443Z\"}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-15 23:03:10'),
(8, NULL, NULL, 'create_user', 'User', 7, NULL, '{ \"email\": \"namkhanh2795@gmail.com\", \"role\": \"admin\" }', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 00:59:09'),
(9, 7, NULL, 'create_announcement', 'Announcement', 2, NULL, '{\"AnnouncementId\":2,\"TeacherId\":7,\"Title\":\"Announcement\",\"Content\":\"Hi\",\"IsActive\":true,\"CreatedAt\":\"2026-05-16T07:59:44.2101316Z\",\"ExpiresAt\":null}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 00:59:44'),
(10, 7, NULL, 'create_user', 'User', 8, NULL, '{ \"email\": \"vuthuy123@gmail.com\", \"role\": \"teacher\" }', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 01:00:23'),
(11, 8, NULL, 'edit_writing_prompt', 'WritingPrompt', 1401, NULL, '{\"PromptId\":1401,\"TaskType\":\"task1\",\"PromptText\":\"The bar chart compares the proportion of household expenditure on transport, food, and housing in three cities in 2010 and 2020. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.\",\"PromptImageUrl\":null,\"ChartType\":\"bar\",\"DifficultyLevel\":3,\"Category\":\"urban economics\",\"TargetBand\":6,\"SampleAnswer\":\"A strong response identifies highest and lowest categories, highlights major shifts, and avoids unsupported causes.\",\"IsActive\":true,\"NotesForTeacher\":\"Check overview quality and data grouping logic.\",\"CreatedAt\":\"2026-04-12T16:42:58\",\"UpdatedAt\":\"2026-05-16T08:47:41\",\"Status\":\"pending_review\",\"CreatedBy\":null,\"ReviewedBy\":null,\"ReviewedAt\":null,\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 01:00:53'),
(12, 8, NULL, 'soft_delete_writing_prompt', 'WritingPrompt', 1401, NULL, '{\"PromptId\":1401,\"TaskType\":\"task1\",\"PromptText\":\"The bar chart compares the proportion of household expenditure on transport, food, and housing in three cities in 2010 and 2020. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.\",\"PromptImageUrl\":null,\"ChartType\":\"bar\",\"DifficultyLevel\":3,\"Category\":\"urban economics\",\"TargetBand\":6,\"SampleAnswer\":\"A strong response identifies highest and lowest categories, highlights major shifts, and avoids unsupported causes.\",\"IsActive\":true,\"NotesForTeacher\":\"Check overview quality and data grouping logic.\",\"CreatedAt\":\"2026-04-12T16:42:58\",\"UpdatedAt\":\"2026-05-16T08:47:41\",\"Status\":\"rejected\",\"CreatedBy\":null,\"ReviewedBy\":null,\"ReviewedAt\":null,\"ReviewerNote\":null,\"IsDeleted\":true,\"Creator\":null,\"Reviewer\":null}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 01:01:04'),
(13, 8, NULL, 'edit_teacher_profile', 'tb_teacher_profiles', 8, '{\"FullName\":\"vuthuy123\",\"AvatarUrl\":null,\"Bio\":null,\"Specialties\":null,\"YearsExperience\":null,\"IsPublic\":null}', '{\"FullName\":\"vuthuy123\",\"AvatarUrl\":\"/uploads/avatars/avatar_8_639145154190129639.jpg\",\"Bio\":\"\\u0026quot;Hello everyone, I am V\\u0169 Th\\u0026#249;y. With over 6 years of specialized experience in teaching and preparing students for the IELTS Academic exam, I have helped more than 1,000 students successfully jump from a 5.0 to a 7.5\\u002B band score.\\r\\n\\r\\nMy teaching philosophy focuses on optimizing linguistic thinking rather than rote memorization of sample essays. At WebIeltsFree, my primary responsibilities include:\\r\\n\\r\\nAuditing and approving new Writing and Speaking practice materials.\\r\\n\\r\\nDirectly grading and resolving disputes regarding AI-generated test results to ensure absolute accuracy.\\r\\n\\r\\nProviding personalized learning roadmaps tailored to each student\\u0026#39;s actual proficiency level.\\r\\n\\r\\nCredentials \\u0026amp; Certifications:\\r\\n\\r\\nIELTS Academic: 8.0 Overall (Listening 8.5, Reading 8.0, Writing 7.5, Speaking 8.0).\\r\\n\\r\\nTESOL International Teaching Certification.\\r\\n\\r\\nBachelor\\u2019s Degree in English Education.\\r\\n\\r\\nI believe that by combining modern AI technology with the practical expertise of our teaching staff, you can conquer your IELTS goals faster and more accurately than ever before.\\u0026quot;\",\"Specialties\":\"8.0 IELTS Certificate\",\"YearsExperience\":6,\"IsPublic\":true}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 01:03:39'),
(14, 8, NULL, 'edit_teacher_profile', 'tb_teacher_profiles', 8, '{\"FullName\":\"vuthuy123\",\"AvatarUrl\":\"/uploads/avatars/avatar_8_639145154190129639.jpg\",\"Bio\":\"\\u0026quot;Hello everyone, I am V\\u0169 Th\\u0026#249;y. With over 6 years of specialized experience in teaching and preparing students for the IELTS Academic exam, I have helped more than 1,000 students successfully jump from a 5.0 to a 7.5\\u002B band score.\\r\\n\\r\\nMy teaching philosophy focuses on optimizing linguistic thinking rather than rote memorization of sample essays. At WebIeltsFree, my primary responsibilities include:\\r\\n\\r\\nAuditing and approving new Writing and Speaking practice materials.\\r\\n\\r\\nDirectly grading and resolving disputes regarding AI-generated test results to ensure absolute accuracy.\\r\\n\\r\\nProviding personalized learning roadmaps tailored to each student\\u0026#39;s actual proficiency level.\\r\\n\\r\\nCredentials \\u0026amp; Certifications:\\r\\n\\r\\nIELTS Academic: 8.0 Overall (Listening 8.5, Reading 8.0, Writing 7.5, Speaking 8.0).\\r\\n\\r\\nTESOL International Teaching Certification.\\r\\n\\r\\nBachelor\\u2019s Degree in English Education.\\r\\n\\r\\nI believe that by combining modern AI technology with the practical expertise of our teaching staff, you can conquer your IELTS goals faster and more accurately than ever before.\\u0026quot;\",\"Specialties\":\"8.0 IELTS Certificate\",\"YearsExperience\":6,\"IsPublic\":true}', '{\"FullName\":\"vuthuy123\",\"AvatarUrl\":\"/uploads/avatars/avatar_8_639145154193219902.jpg\",\"Bio\":\"\\u0026quot;Hello everyone, I am V\\u0169 Th\\u0026#249;y. With over 6 years of specialized experience in teaching and preparing students for the IELTS Academic exam, I have helped more than 1,000 students successfully jump from a 5.0 to a 7.5\\u002B band score.\\r\\n\\r\\nMy teaching philosophy focuses on optimizing linguistic thinking rather than rote memorization of sample essays. At WebIeltsFree, my primary responsibilities include:\\r\\n\\r\\nAuditing and approving new Writing and Speaking practice materials.\\r\\n\\r\\nDirectly grading and resolving disputes regarding AI-generated test results to ensure absolute accuracy.\\r\\n\\r\\nProviding personalized learning roadmaps tailored to each student\\u0026#39;s actual proficiency level.\\r\\n\\r\\nCredentials \\u0026amp; Certifications:\\r\\n\\r\\nIELTS Academic: 8.0 Overall (Listening 8.5, Reading 8.0, Writing 7.5, Speaking 8.0).\\r\\n\\r\\nTESOL International Teaching Certification.\\r\\n\\r\\nBachelor\\u2019s Degree in English Education.\\r\\n\\r\\nI believe that by combining modern AI technology with the practical expertise of our teaching staff, you can conquer your IELTS goals faster and more accurately than ever before.\\u0026quot;\",\"Specialties\":\"8.0 IELTS Certificate\",\"YearsExperience\":6,\"IsPublic\":true}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 01:03:39'),
(15, 8, NULL, 'approve_content', 'SpeakingTopic', 1, NULL, '{\"TopicId\":1,\"PartNumber\":2,\"TopicName\":\"A memorable trip\",\"Description\":\"Describe a memorable trip you had in your life.\",\"DifficultyLevel\":2,\"TargetBand\":6,\"IsActive\":true,\"CreatedAt\":\"2026-04-12T11:33:21\",\"UpdatedAt\":\"2026-05-16T09:17:53\",\"Status\":\"published\",\"CreatedBy\":null,\"ReviewedBy\":8,\"ReviewedAt\":\"2026-05-16T08:06:51.2968204Z\",\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null,\"Parts\":[]}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 01:06:51'),
(16, 8, NULL, 'suggest_roadmap_change', 'RoadmapSuggestion', 2, NULL, '{\"SuggestionId\":2,\"TeacherId\":8,\"StudentId\":3,\"RoadmapId\":3,\"SuggestionTitle\":\"Add more writing example\",\"Message\":\"aa\",\"SuggestedChanges\":\"[{\\u0022action\\u0022:\\u0022add\\u0022,\\u0022week_number\\u0022:2,\\u0022description\\u0022:\\u0022a\\u0022}]\",\"Status\":\"pending\",\"CreatedAt\":\"2026-05-16T09:51:05.376459Z\",\"ResolvedAt\":null,\"Teacher\":null,\"Student\":null,\"Roadmap\":null}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 02:51:05'),
(17, 7, NULL, 'toggle_ban', 'User', 1, '{ \"status\": \"active\" }', '{ \"status\": \"banned\" }', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 08:47:39'),
(18, 8, NULL, 'edit_writing_prompt', 'WritingPrompt', 1, NULL, '{\"PromptId\":1,\"TaskType\":\"task1\",\"PromptText\":\"The chart below shows energy usage by sector from 2000 to 2020.\",\"PromptImageUrl\":\"/uploads/writing/0a040568-d58f-4606-9c4b-dbc6bd39fac4.png\",\"ChartType\":\"line\",\"DifficultyLevel\":2,\"Category\":\"energy\",\"TargetBand\":6,\"SampleAnswer\":\"Sample response text...\",\"IsActive\":true,\"NotesForTeacher\":\"Ask student to compare trends.\",\"CreatedAt\":\"2026-04-12T11:33:21\",\"UpdatedAt\":\"2026-04-12T11:33:21\",\"Status\":\"draft\",\"CreatedBy\":null,\"ReviewedBy\":null,\"ReviewedAt\":null,\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 08:48:38'),
(19, 8, NULL, 'edit_writing_prompt', 'WritingPrompt', 1, NULL, '{\"PromptId\":1,\"TaskType\":\"task1\",\"PromptText\":\"The chart below shows energy usage by sector from 2000 to 2020.\",\"PromptImageUrl\":\"/uploads/writing/0a040568-d58f-4606-9c4b-dbc6bd39fac4.png\",\"ChartType\":\"line\",\"DifficultyLevel\":2,\"Category\":\"energy\",\"TargetBand\":6,\"SampleAnswer\":\"Sample response text...\",\"IsActive\":true,\"NotesForTeacher\":\"Ask student to compare trends.\",\"CreatedAt\":\"2026-04-12T11:33:21\",\"UpdatedAt\":\"2026-05-16T22:48:38\",\"Status\":\"pending_review\",\"CreatedBy\":null,\"ReviewedBy\":null,\"ReviewedAt\":null,\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 08:48:49'),
(20, 8, NULL, 'approve_content', 'WritingPrompt', 1, NULL, '{\"PromptId\":1,\"TaskType\":\"task1\",\"PromptText\":\"The chart below shows energy usage by sector from 2000 to 2020.\",\"PromptImageUrl\":\"/uploads/writing/0a040568-d58f-4606-9c4b-dbc6bd39fac4.png\",\"ChartType\":\"line\",\"DifficultyLevel\":2,\"Category\":\"energy\",\"TargetBand\":6,\"SampleAnswer\":\"Sample response text...\",\"IsActive\":true,\"NotesForTeacher\":\"Ask student to compare trends.\",\"CreatedAt\":\"2026-04-12T11:33:21\",\"UpdatedAt\":\"2026-05-16T22:48:49\",\"Status\":\"published\",\"CreatedBy\":null,\"ReviewedBy\":8,\"ReviewedAt\":\"2026-05-16T15:48:53.300355Z\",\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 08:48:53'),
(21, 8, NULL, 'create_assignment', 'TeacherAssignment', 3, NULL, '{\"AssignmentId\":3,\"TestId\":2004,\"TeacherId\":8,\"StudentId\":9,\"Title\":\"a\",\"Instructions\":\"a\",\"Deadline\":null,\"Status\":\"pending\",\"AssignedAt\":\"2026-05-16T15:49:14.9522398Z\",\"CompletedAt\":null,\"AttemptId\":null,\"IsDeleted\":false,\"Teacher\":null,\"Student\":null,\"Test\":null}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 08:49:14'),
(22, 8, NULL, 'edit_assignment', 'TeacherAssignment', 3, NULL, '{\"AssignmentId\":3,\"TestId\":2004,\"TeacherId\":8,\"StudentId\":9,\"Title\":\"a\",\"Instructions\":\"a\",\"Deadline\":null,\"Status\":\"pending\",\"AssignedAt\":\"2026-05-16T15:49:14\",\"CompletedAt\":null,\"AttemptId\":null,\"IsDeleted\":false,\"Teacher\":null,\"Student\":null,\"Test\":null}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 08:49:19'),
(23, 8, NULL, 'edit_speaking_topic', 'SpeakingTopic', 2002, NULL, '{\"TopicId\":2002,\"PartNumber\":2,\"TopicName\":\"A time you helped someone\",\"Description\":\"Describe a time when you helped someone who really needed it. You should say who the person was, what the situation was, how you helped, and explain how you felt afterwards.\",\"DifficultyLevel\":3,\"TargetBand\":7,\"IsActive\":true,\"CreatedAt\":\"2026-05-16T22:45:17\",\"UpdatedAt\":\"2026-05-16T22:45:17\",\"Status\":\"pending_review\",\"CreatedBy\":null,\"ReviewedBy\":null,\"ReviewedAt\":null,\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null,\"Parts\":[]}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 08:51:57'),
(24, 8, NULL, 'approve_content', 'SpeakingTopic', 2002, NULL, '{\"TopicId\":2002,\"PartNumber\":2,\"TopicName\":\"A time you helped someone\",\"Description\":\"Describe a time when you helped someone who really needed it. You should say who the person was, what the situation was, how you helped, and explain how you felt afterwards.\",\"DifficultyLevel\":3,\"TargetBand\":7,\"IsActive\":true,\"CreatedAt\":\"2026-05-16T22:45:17\",\"UpdatedAt\":\"2026-05-16T22:51:57\",\"Status\":\"published\",\"CreatedBy\":null,\"ReviewedBy\":8,\"ReviewedAt\":\"2026-05-16T15:52:02.4836175Z\",\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null,\"Parts\":[]}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 08:52:02'),
(25, 8, NULL, 'edit_speaking_topic', 'SpeakingTopic', 2002, NULL, '{\"TopicId\":2002,\"PartNumber\":2,\"TopicName\":\"A time you helped someone\",\"Description\":\"Describe a time when you helped someone who really needed it. You should say who the person was, what the situation was, how you helped, and explain how you felt afterwards.\",\"DifficultyLevel\":3,\"TargetBand\":7,\"IsActive\":true,\"CreatedAt\":\"2026-05-16T22:45:17\",\"UpdatedAt\":\"2026-05-16T22:52:02\",\"Status\":\"pending_review\",\"CreatedBy\":null,\"ReviewedBy\":8,\"ReviewedAt\":\"2026-05-16T15:52:02\",\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null,\"Parts\":[]}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 08:52:09'),
(26, 8, NULL, 'approve_content', 'SpeakingTopic', 2002, NULL, '{\"TopicId\":2002,\"PartNumber\":2,\"TopicName\":\"A time you helped someone\",\"Description\":\"Describe a time when you helped someone who really needed it. You should say who the person was, what the situation was, how you helped, and explain how you felt afterwards.\",\"DifficultyLevel\":3,\"TargetBand\":7,\"IsActive\":true,\"CreatedAt\":\"2026-05-16T22:45:17\",\"UpdatedAt\":\"2026-05-16T22:52:09\",\"Status\":\"published\",\"CreatedBy\":null,\"ReviewedBy\":8,\"ReviewedAt\":\"2026-05-16T15:52:13.4573202Z\",\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null,\"Parts\":[]}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 08:52:13'),
(27, 8, NULL, 'edit_speaking_topic', 'SpeakingTopic', 1502, NULL, '{\"TopicId\":1502,\"PartNumber\":1,\"TopicName\":\"Daily routines and study/work habits\",\"Description\":\"Describe your regular weekday routine and how you manage your schedule.\",\"DifficultyLevel\":2,\"TargetBand\":6,\"IsActive\":true,\"CreatedAt\":\"2026-04-12T16:42:58\",\"UpdatedAt\":\"2026-04-12T16:42:58\",\"Status\":\"pending_review\",\"CreatedBy\":null,\"ReviewedBy\":null,\"ReviewedAt\":null,\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null,\"Parts\":[]}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 08:52:20'),
(28, 8, NULL, 'edit_speaking_topic', 'SpeakingTopic', 1502, NULL, '{\"TopicId\":1502,\"PartNumber\":1,\"TopicName\":\"Daily routines and study/work habits\",\"Description\":\"Describe your regular weekday routine and how you manage your schedule.\",\"DifficultyLevel\":2,\"TargetBand\":6,\"IsActive\":true,\"CreatedAt\":\"2026-04-12T16:42:58\",\"UpdatedAt\":\"2026-05-16T22:52:20\",\"Status\":\"pending_review\",\"CreatedBy\":null,\"ReviewedBy\":null,\"ReviewedAt\":null,\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null,\"Parts\":[]}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 08:52:28'),
(29, 8, NULL, 'approve_content', 'SpeakingTopic', 1502, NULL, '{\"TopicId\":1502,\"PartNumber\":1,\"TopicName\":\"Daily routines and study/work habits\",\"Description\":\"Describe your regular weekday routine and how you manage your schedule.\",\"DifficultyLevel\":2,\"TargetBand\":6,\"IsActive\":true,\"CreatedAt\":\"2026-04-12T16:42:58\",\"UpdatedAt\":\"2026-05-16T22:52:20\",\"Status\":\"published\",\"CreatedBy\":null,\"ReviewedBy\":8,\"ReviewedAt\":\"2026-05-16T15:52:30.7737686Z\",\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null,\"Parts\":[]}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 08:52:30'),
(30, 8, NULL, 'edit_speaking_topic', 'SpeakingTopic', 1506, NULL, '{\"TopicId\":1506,\"PartNumber\":2,\"TopicName\":\"A place that became popular in your city\",\"Description\":\"Describe a place that has become popular recently in your city and explain why people like it.\",\"DifficultyLevel\":3,\"TargetBand\":6,\"IsActive\":true,\"CreatedAt\":\"2026-04-12T16:42:58\",\"UpdatedAt\":\"2026-04-12T16:42:58\",\"Status\":\"pending_review\",\"CreatedBy\":null,\"ReviewedBy\":null,\"ReviewedAt\":null,\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null,\"Parts\":[]}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 08:52:41'),
(31, 8, NULL, 'approve_content', 'SpeakingTopic', 1506, NULL, '{\"TopicId\":1506,\"PartNumber\":2,\"TopicName\":\"A place that became popular in your city\",\"Description\":\"Describe a place that has become popular recently in your city and explain why people like it.\",\"DifficultyLevel\":3,\"TargetBand\":6,\"IsActive\":true,\"CreatedAt\":\"2026-04-12T16:42:58\",\"UpdatedAt\":\"2026-05-16T22:52:41\",\"Status\":\"published\",\"CreatedBy\":null,\"ReviewedBy\":8,\"ReviewedAt\":\"2026-05-16T15:52:47.5943343Z\",\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null,\"Parts\":[]}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 08:52:47'),
(32, 8, NULL, 'edit_speaking_topic', 'SpeakingTopic', 1510, NULL, '{\"TopicId\":1510,\"PartNumber\":3,\"TopicName\":\"Urban transport and quality of life\",\"Description\":\"Discuss how transport planning affects productivity, health, and equality.\",\"DifficultyLevel\":3,\"TargetBand\":7,\"IsActive\":true,\"CreatedAt\":\"2026-04-12T16:42:58\",\"UpdatedAt\":\"2026-04-12T16:42:58\",\"Status\":\"pending_review\",\"CreatedBy\":null,\"ReviewedBy\":null,\"ReviewedAt\":null,\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null,\"Parts\":[]}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 08:52:56'),
(33, 8, NULL, 'reject_content', 'SpeakingTopic', 1510, NULL, '{\"TopicId\":1510,\"PartNumber\":3,\"TopicName\":\"Urban transport and quality of life\",\"Description\":\"Discuss how transport planning affects productivity, health, and equality.\",\"DifficultyLevel\":3,\"TargetBand\":7,\"IsActive\":true,\"CreatedAt\":\"2026-04-12T16:42:58\",\"UpdatedAt\":\"2026-05-16T22:52:56\",\"Status\":\"rejected\",\"CreatedBy\":null,\"ReviewedBy\":8,\"ReviewedAt\":\"2026-05-16T15:53:05.3125468Z\",\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null,\"Parts\":[]}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 08:53:05'),
(34, 7, NULL, 'update_report_status', 'Report', 1, '{ \"status\": \"open\" }', '{ \"status\": \"resolved\" }', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 09:36:48'),
(35, 8, NULL, 'edit_assignment', 'TeacherAssignment', 3, NULL, '{\"AssignmentId\":3,\"TestId\":2004,\"TeacherId\":8,\"StudentId\":9,\"Title\":\"a\",\"Instructions\":\"a\",\"Deadline\":null,\"Status\":\"pending\",\"AssignedAt\":\"2026-05-16T15:49:14\",\"CompletedAt\":null,\"AttemptId\":null,\"IsDeleted\":false,\"Teacher\":null,\"Student\":null,\"Test\":null}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 09:42:42'),
(36, 8, NULL, 'edit_speaking_topic', 'SpeakingTopic', 1503, NULL, '{\"TopicId\":1503,\"PartNumber\":1,\"TopicName\":\"Leisure and media preferences\",\"Description\":\"Discuss your free-time activities and what media you consume.\",\"DifficultyLevel\":2,\"TargetBand\":6,\"IsActive\":true,\"CreatedAt\":\"2026-04-12T16:42:58\",\"UpdatedAt\":\"2026-04-12T16:42:58\",\"Status\":\"pending_review\",\"CreatedBy\":null,\"ReviewedBy\":null,\"ReviewedAt\":null,\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null,\"Parts\":[]}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 09:45:14'),
(37, 7, NULL, 'update_suggestion_status', 'RoadmapSuggestion', 2, '{ \"status\": \"accepted\" }', '{ \"status\": \"pending\" }', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 09:51:26'),
(38, 7, NULL, 'force_apply_suggestion', 'RoadmapSuggestion', 2, '{ \"status\": \"pending\" }', '{ \"status\": \"applied\" }', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 09:51:33'),
(39, 7, NULL, 'update_suggestion_status', 'RoadmapSuggestion', 2, '{ \"status\": \"\" }', '{ \"status\": \"approved\" }', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-16 09:51:47'),
(40, 7, NULL, 'create_course', 'Course', 2002, NULL, '{\"Title\":\"Full Course for 4.0 - 5.0 Speaking Skill\",\"SkillType\":\"speaking\"}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-18 06:53:02'),
(41, 7, NULL, 'close_report', 'Report', 1, '{ \"status\": \"resolved\" }', '{ \"status\": \"closed\" }', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-18 06:53:56'),
(42, 8, NULL, 'approve_content', 'SpeakingTopic', 1503, NULL, '{\"TopicId\":1503,\"PartNumber\":1,\"TopicName\":\"Leisure and media preferences\",\"Description\":\"Discuss your free-time activities and what media you consume.\",\"DifficultyLevel\":2,\"TargetBand\":6,\"IsActive\":true,\"CreatedAt\":\"2026-04-12T16:42:58\",\"UpdatedAt\":\"2026-05-16T23:45:14\",\"Status\":\"published\",\"CreatedBy\":null,\"ReviewedBy\":8,\"ReviewedAt\":\"2026-05-20T16:30:41.4929501Z\",\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null,\"Parts\":[]}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-20 09:30:41'),
(43, 8, NULL, 'edit_teacher_profile', 'tb_teacher_profiles', 8, '{\"FullName\":\"vuthuy123\",\"AvatarUrl\":\"/uploads/avatars/avatar_8_639145154193219902.jpg\",\"Bio\":\"\\u0026quot;Hello everyone, I am V\\u0169 Th\\u0026#249;y. With over 6 years of specialized experience in teaching and preparing students for the IELTS Academic exam, I have helped more than 1,000 students successfully jump from a 5.0 to a 7.5\\u002B band score.\\r\\n\\r\\nMy teaching philosophy focuses on optimizing linguistic thinking rather than rote memorization of sample essays. At WebIeltsFree, my primary responsibilities include:\\r\\n\\r\\nAuditing and approving new Writing and Speaking practice materials.\\r\\n\\r\\nDirectly grading and resolving disputes regarding AI-generated test results to ensure absolute accuracy.\\r\\n\\r\\nProviding personalized learning roadmaps tailored to each student\\u0026#39;s actual proficiency level.\\r\\n\\r\\nCredentials \\u0026amp; Certifications:\\r\\n\\r\\nIELTS Academic: 8.0 Overall (Listening 8.5, Reading 8.0, Writing 7.5, Speaking 8.0).\\r\\n\\r\\nTESOL International Teaching Certification.\\r\\n\\r\\nBachelor\\u2019s Degree in English Education.\\r\\n\\r\\nI believe that by combining modern AI technology with the practical expertise of our teaching staff, you can conquer your IELTS goals faster and more accurately than ever before.\\u0026quot;\",\"Specialties\":\"8.0 IELTS Certificate\",\"YearsExperience\":6,\"IsPublic\":true}', '{\"FullName\":\"vuthuy123\",\"AvatarUrl\":\"/uploads/avatars/avatar_8_639148914586623579.svg\",\"Bio\":\"\\u0026amp;quot;Hello everyone, I am V\\u0169 Th\\u0026amp;#249;y. With over 6 years of specialized experience in teaching and preparing students for the IELTS Academic exam, I have helped more than 1,000 students successfully jump from a 5.0 to a 7.5\\u002B band score.\\r\\n\\r\\nMy teaching philosophy focuses on optimizing linguistic thinking rather than rote memorization of sample essays. At WebIeltsFree, my primary responsibilities include:\\r\\n\\r\\nAuditing and approving new Writing and Speaking practice materials.\\r\\n\\r\\nDirectly grading and resolving disputes regarding AI-generated test results to ensure absolute accuracy.\\r\\n\\r\\nProviding personalized learning roadmaps tailored to each student\\u0026amp;#39;s actual proficiency level.\\r\\n\\r\\nCredentials \\u0026amp;amp; Certifications:\\r\\n\\r\\nIELTS Academic: 8.0 Overall (Listening 8.5, Reading 8.0, Writing 7.5, Speaking 8.0).\\r\\n\\r\\nTESOL International Teaching Certification.\\r\\n\\r\\nBachelor\\u2019s Degree in English Education.\\r\\n\\r\\nI believe that by combining modern AI technology with the practical expertise of our teaching staff, you can conquer your IELTS goals faster and more accurately than ever before.\\u0026amp;quot;\",\"Specialties\":\"8.0 IELTS Certificate\",\"YearsExperience\":6,\"IsPublic\":true}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-20 09:30:58'),
(44, 8, NULL, 'create_writing_prompt', 'WritingPrompt', 2005, NULL, '{\"PromptId\":2005,\"TaskType\":\"task1\",\"PromptText\":\"a\",\"PromptImageUrl\":null,\"ChartType\":null,\"DifficultyLevel\":1,\"Category\":null,\"TargetBand\":6,\"SampleAnswer\":\"a\",\"IsActive\":true,\"NotesForTeacher\":null,\"CreatedAt\":\"2026-05-20T16:45:17.5154004Z\",\"UpdatedAt\":null,\"Status\":\"draft\",\"CreatedBy\":8,\"ReviewedBy\":null,\"ReviewedAt\":null,\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-20 09:45:17'),
(45, 8, NULL, 'soft_delete_writing_prompt', 'WritingPrompt', 2005, NULL, '{\"PromptId\":2005,\"TaskType\":\"task1\",\"PromptText\":\"a\",\"PromptImageUrl\":null,\"ChartType\":null,\"DifficultyLevel\":1,\"Category\":null,\"TargetBand\":6,\"SampleAnswer\":\"a\",\"IsActive\":true,\"NotesForTeacher\":null,\"CreatedAt\":\"2026-05-20T16:45:17\",\"UpdatedAt\":\"2026-05-20T23:45:17\",\"Status\":\"rejected\",\"CreatedBy\":8,\"ReviewedBy\":null,\"ReviewedAt\":null,\"ReviewerNote\":null,\"IsDeleted\":true,\"Creator\":null,\"Reviewer\":null}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-20 09:45:23'),
(46, 8, NULL, 'create_speaking_topic', 'SpeakingTopic', 2004, NULL, '{\"TopicId\":2004,\"PartNumber\":2,\"TopicName\":\"Describe a best journey you\\u0026#39;ve had\",\"Description\":null,\"DifficultyLevel\":1,\"TargetBand\":5,\"IsActive\":true,\"CreatedAt\":\"2026-05-20T16:45:58.7607568Z\",\"UpdatedAt\":null,\"Status\":\"draft\",\"CreatedBy\":8,\"ReviewedBy\":null,\"ReviewedAt\":null,\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null,\"Parts\":[]}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-20 09:45:58'),
(47, 8, NULL, 'edit_speaking_topic', 'SpeakingTopic', 2004, NULL, '{\"TopicId\":2004,\"PartNumber\":2,\"TopicName\":\"Describe a best journey you\\u0026amp;#39;ve had\",\"Description\":null,\"DifficultyLevel\":1,\"TargetBand\":5,\"IsActive\":true,\"CreatedAt\":\"2026-05-20T16:45:58\",\"UpdatedAt\":\"2026-05-20T23:45:58\",\"Status\":\"pending_review\",\"CreatedBy\":8,\"ReviewedBy\":null,\"ReviewedAt\":null,\"ReviewerNote\":null,\"IsDeleted\":false,\"Creator\":null,\"Reviewer\":null,\"Parts\":[]}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-20 09:46:04'),
(48, 8, NULL, 'create_assignment', 'TeacherAssignment', 4, NULL, '{\"AssignmentId\":4,\"TestId\":2001,\"TeacherId\":8,\"StudentId\":9,\"Title\":\"Weekly\",\"Instructions\":\"aaa\",\"Deadline\":\"2026-05-21T01:00:00\",\"Status\":\"pending\",\"AssignedAt\":\"2026-05-21T04:13:47.6574436Z\",\"CompletedAt\":null,\"AttemptId\":null,\"IsDeleted\":false,\"Teacher\":null,\"Student\":null,\"Test\":null,\"Attempt\":null}', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'success', NULL, '2026-05-20 21:13:47'),
(49, NULL, NULL, 'mark_assignment_overdue', 'TeacherAssignment', 4, '{\"status\":\"pending\"}', '{\"status\":\"overdue\"}', 'system', 'OverdueAssignmentService', 'success', NULL, '2026-05-20 21:21:58');

-- --------------------------------------------------------

--
-- Table structure for table `tb_conversations`
--

CREATE TABLE `tb_conversations` (
  `conversation_id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `teacher_id` int(11) DEFAULT NULL,
  `subject` varchar(255) DEFAULT NULL,
  `status` enum('open','active','closed') NOT NULL DEFAULT 'open',
  `created_at` datetime DEFAULT current_timestamp(),
  `last_message_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tb_courses`
--

CREATE TABLE `tb_courses` (
  `course_id` int(11) NOT NULL,
  `title` varchar(255) DEFAULT NULL,
  `target_band` float DEFAULT NULL,
  `skill_type` enum('reading','listening','writing','speaking','vocabulary','grammar','mixed') DEFAULT 'mixed',
  `difficulty_level` int(11) DEFAULT 1,
  `thumbnail_url` varchar(500) DEFAULT NULL,
  `estimated_hours` int(11) DEFAULT NULL,
  `is_published` tinyint(1) DEFAULT 0,
  `order_index` int(11) DEFAULT NULL,
  `slug` varchar(255) DEFAULT NULL,
  `description` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_courses`
--

INSERT INTO `tb_courses` (`course_id`, `title`, `target_band`, `skill_type`, `difficulty_level`, `thumbnail_url`, `estimated_hours`, `is_published`, `order_index`, `slug`, `description`) VALUES
(1, 'IELTS Reading Excellence', 7, 'reading', 2, 'https://example.com/reading.jpg', 40, 1, 1, 'ielts-reading-excellence', 'Reading strategy and question type mastery'),
(2, 'IELTS Listening Mastery', 7, 'listening', 2, 'https://example.com/listening.jpg', 35, 1, 2, 'ielts-listening-mastery', 'Listening skills across all IELTS parts'),
(3, 'IELTS Writing Perfection', 7, 'writing', 3, 'https://example.com/writing.jpg', 45, 1, 3, 'ielts-writing-perfection', 'Task 1 and Task 2 practice'),
(4, 'IELTS Speaking Confidence', 7, 'speaking', 2, 'https://example.com/speaking.jpg', 30, 1, 4, 'ielts-speaking-confidence', 'Speaking fluency and coherence'),
(201, 'IELTS Listening Academic 4-Part Mastery', 7, 'listening', 3, 'https://example.com/img/listening-academic-4part.jpg', 45, 1, 21, 'ielts-listening-academic-4-part-mastery', 'Listening pathway aligned with official 4-part progression: social contexts in Parts 1-2 and academic contexts in Parts 3-4.'),
(202, 'IELTS Reading Academic 3-Passage Intensive', 7, 'reading', 3, 'https://example.com/img/reading-academic-3passage.jpg', 48, 1, 22, 'ielts-reading-academic-3-passage-intensive', 'Reading pathway following 3-passage academic format with timing control and high-frequency question types.'),
(203, 'IELTS Writing Academic Task 1 and Task 2', 7, 'writing', 3, 'https://example.com/img/writing-academic.jpg', 52, 1, 23, 'ielts-writing-academic-task1-task2', 'Writing track based on official task split, word-count thresholds, and descriptor-driven feedback.'),
(204, 'IELTS Speaking 3-Part Examiner Simulation', 7, 'speaking', 3, 'https://example.com/img/speaking-3part.jpg', 40, 1, 24, 'ielts-speaking-3-part-examiner-simulation', 'Speaking track covering Part 1 interview, Part 2 long turn, and Part 3 analytical discussion with AI-assisted review.'),
(205, 'IELTS Academic Full Test Simulation', 7.5, 'mixed', 4, 'https://example.com/img/ielts-full-sim.jpg', 60, 1, 25, 'ielts-academic-full-test-simulation', 'Integrated full-test simulation with realistic section timing and answer workflow.'),
(2001, 'Cambridge IELTS 20 - Academic Test 1', 7, 'mixed', 3, '/images/cam20.jpg', 3, 1, 30, 'cambridge-ielts-20-test-1', 'Full academic practice test from Cambridge IELTS 20 with Listening, Reading, Writing and Speaking.'),
(2002, 'Full Course for 4.0 - 5.0 Speaking Skill', 5, 'speaking', 3, NULL, 10, 1, 10, 'full-course-for-4.0---5.0-speaking-skill', 'For whose are having problems with level up your band from 4.0 to 5.0');

-- --------------------------------------------------------

--
-- Table structure for table `tb_gamification`
--

CREATE TABLE `tb_gamification` (
  `achievement_id` int(11) NOT NULL,
  `achievement_code` varchar(100) NOT NULL,
  `achievement_name` varchar(255) NOT NULL,
  `description` text DEFAULT NULL,
  `icon_url` varchar(500) DEFAULT NULL,
  `points` int(11) DEFAULT 0,
  `badge_color` varchar(20) DEFAULT NULL,
  `criteria_json` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`criteria_json`)),
  `is_active` tinyint(1) DEFAULT 1,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_gamification`
--

INSERT INTO `tb_gamification` (`achievement_id`, `achievement_code`, `achievement_name`, `description`, `icon_url`, `points`, `badge_color`, `criteria_json`, `is_active`, `created_at`) VALUES
(1, 'FIRST_LESSON', 'First Lesson Completed', 'Complete your first lesson', 'https://example.com/icons/first-lesson.png', 50, 'gold', '{"type":"lesson_completed","count":1}', 1, '2026-04-12 04:33:21'),
(2, 'VOCAB_NOVICE', 'Vocabulary Novice', 'Learn 5 new vocabulary words', 'https://example.com/icons/vocab-novice.png', 100, 'blue', '{"type":"vocab_learned","count":5}', 1, '2026-05-26 10:00:00'),
(3, 'VOCAB_MASTER', 'Vocabulary Master', 'Learn 20 new vocabulary words', 'https://example.com/icons/vocab-master.png', 300, 'purple', '{"type":"vocab_learned","count":20}', 1, '2026-05-26 10:00:00'),
(4, 'SPEAKING_STARTER', 'Speaking Starter', 'Record your first speaking response', 'https://example.com/icons/speaking-starter.png', 100, 'green', '{"type":"speaking_session","count":1}', 1, '2026-05-26 10:00:00'),
(5, 'SPEAKING_PRO', 'Speaking Professional', 'Achieve a Band 7.0+ in any speaking session', 'https://example.com/icons/speaking-pro.png', 500, 'red', '{"type":"speaking_band","min_score":7.0}', 1, '2026-05-26 10:00:00'),
(6, 'WRITING_BEGINNER', 'Writing Beginner', 'Submit your first essay for AI evaluation', 'https://example.com/icons/writing-beginner.png', 100, 'orange', '{"type":"writing_submission","count":1}', 1, '2026-05-26 10:00:00'),
(7, 'WRITING_CHAMPION', 'Writing Champion', 'Achieve a Band 7.5+ in any writing prompt', 'https://example.com/icons/writing-champion.png', 500, 'platinum', '{"type":"writing_band","min_score":7.5}', 1, '2026-05-26 10:00:00'),
(8, 'READING_ACE', 'Reading Ace', 'Answer 10 reading practice questions correctly', 'https://example.com/icons/reading-ace.png', 200, 'bronze', '{"type":"reading_correct","count":10}', 1, '2026-05-26 10:00:00'),
(9, 'LISTENING_WIZARD', 'Listening Wizard', 'Answer 10 listening practice questions correctly', 'https://example.com/icons/listening-wizard.png', 200, 'cyan', '{"type":"listening_correct","count":10}', 1, '2026-05-26 10:00:00'),
(10, 'DAILY_STREAK_7', 'Weekly Warrior', 'Maintain a 7-day study streak', 'https://example.com/icons/streak-7.png', 400, 'silver', '{"type":"streak_days","count":7}', 1, '2026-05-26 10:00:00');

-- --------------------------------------------------------

--
-- Table structure for table `tb_grade_disputes`
--

CREATE TABLE `tb_grade_disputes` (
  `dispute_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `submission_type` enum('writing','speaking') NOT NULL,
  `submission_id` int(11) NOT NULL,
  `reason` text NOT NULL,
  `status` enum('pending','under_review','resolved','rejected') NOT NULL DEFAULT 'pending',
  `reviewed_by` int(11) DEFAULT NULL,
  `original_score` float NOT NULL,
  `revised_score` float DEFAULT NULL,
  `teacher_notes` text DEFAULT NULL,
  `created_at` datetime DEFAULT current_timestamp(),
  `resolved_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tb_lessons`
--

CREATE TABLE `tb_lessons` (
  `lesson_id` int(11) NOT NULL,
  `module_id` int(11) DEFAULT NULL,
  `title` varchar(255) DEFAULT NULL,
  `skill_type` enum('reading','listening','writing','speaking') DEFAULT NULL,
  `difficulty_level` int(11) DEFAULT NULL,
  `estimated_minutes` int(11) DEFAULT NULL,
  `is_deleted` tinyint(1) DEFAULT 0,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_lessons`
--

INSERT INTO `tb_lessons` (`lesson_id`, `module_id`, `title`, `skill_type`, `difficulty_level`, `estimated_minutes`, `is_deleted`, `deleted_at`, `updated_at`) VALUES
(1, 1, 'TFNG Foundations', 'reading', 2, 30, 0, NULL, '2026-04-12 04:33:20'),
(2, 2, 'Listening Notes and Forms', 'listening', 2, 30, 0, NULL, '2026-04-12 04:33:20'),
(3, 3, 'Describe Line Chart', 'writing', 3, 40, 0, NULL, '2026-04-12 04:33:20'),
(4, 4, 'Personal Questions Drill', 'speaking', 2, 25, 0, NULL, '2026-04-12 04:33:20'),
(401, 301, 'Form Completion with Names, Numbers, and Dates', 'listening', 2, 30, 0, NULL, '2026-04-12 09:42:57'),
(402, 301, 'Section 1 Distractor Patterns and Correction Traps', 'listening', 3, 35, 0, NULL, '2026-04-12 09:42:57'),
(403, 302, 'Map and Plan Labelling with Directional Language', 'listening', 3, 35, 0, NULL, '2026-04-12 09:42:57'),
(404, 302, 'Table and Note Completion in Public Information Talks', 'listening', 3, 35, 0, NULL, '2026-04-12 09:42:57'),
(405, 303, 'Matching Speakers to Opinions in Seminar Dialogues', 'listening', 3, 35, 0, NULL, '2026-04-12 09:42:57'),
(406, 303, 'Academic MCQ under Limited Replay Conditions', 'listening', 4, 40, 0, NULL, '2026-04-12 09:42:57'),
(407, 304, 'Lecture Signposting and Summary Completion', 'listening', 4, 40, 0, NULL, '2026-04-12 09:42:57'),
(408, 304, 'High-speed Note Completion for Part 4', 'listening', 4, 40, 0, NULL, '2026-04-12 09:42:57'),
(409, 305, 'Skimming and Scanning for Passage 1', 'reading', 2, 35, 0, NULL, '2026-04-12 09:42:57'),
(410, 305, 'True/False/Not Given Evidence Mapping', 'reading', 3, 40, 0, NULL, '2026-04-12 09:42:57'),
(411, 306, 'Matching Headings and Information Blocks', 'reading', 3, 40, 0, NULL, '2026-04-12 09:42:57'),
(412, 306, 'Sentence Completion with Word Limits', 'reading', 3, 40, 0, NULL, '2026-04-12 09:42:57'),
(413, 307, 'Inference-heavy Multiple Choice', 'reading', 4, 45, 0, NULL, '2026-04-12 09:42:57'),
(414, 307, 'Summary Completion and Paraphrase Tracking', 'reading', 4, 45, 0, NULL, '2026-04-12 09:42:57'),
(415, 308, 'Task 1 Overview-first Writing Framework', 'writing', 3, 45, 0, NULL, '2026-04-12 09:42:57'),
(416, 308, 'Line/Bar/Pie/Table Comparative Reporting', 'writing', 3, 45, 0, NULL, '2026-04-12 09:42:57'),
(417, 309, 'Process Diagram Sequencing and Passive Forms', 'writing', 3, 45, 0, NULL, '2026-04-12 09:42:57'),
(418, 309, 'Map Changes Across Time and Spatial Grouping', 'writing', 3, 45, 0, NULL, '2026-04-12 09:42:57'),
(419, 310, 'Opinion and Discussion Essay Architecture', 'writing', 3, 50, 0, NULL, '2026-04-12 09:42:57'),
(420, 310, 'Coherence, Cohesion, and Paragraph Logic', 'writing', 4, 50, 0, NULL, '2026-04-12 09:42:57'),
(421, 311, 'Problem-Solution Essays with Feasible Proposals', 'writing', 4, 50, 0, NULL, '2026-04-12 09:42:57'),
(422, 311, 'Advantages-Disadvantages with Balanced Position', 'writing', 4, 50, 0, NULL, '2026-04-12 09:42:57'),
(423, 312, 'Part 1 Natural Expansion without Memorised Chunks', 'speaking', 2, 30, 0, NULL, '2026-04-12 09:42:57'),
(424, 312, 'Fluency Control and Pronunciation Clarity', 'speaking', 3, 35, 0, NULL, '2026-04-12 09:42:57'),
(425, 313, '1-minute Planning for Cue Card Delivery', 'speaking', 3, 35, 0, NULL, '2026-04-12 09:42:57'),
(426, 313, '2-minute Long Turn with Strong Storyline', 'speaking', 3, 35, 0, NULL, '2026-04-12 09:42:57'),
(427, 314, 'Developing Abstract Ideas in Part 3', 'speaking', 4, 35, 0, NULL, '2026-04-12 09:42:57'),
(428, 314, 'Compare-Cause-Predict Framework for Discussion', 'speaking', 4, 35, 0, NULL, '2026-04-12 09:42:57'),
(429, 315, 'Listening and Reading Time Allocation Blueprint', '', 3, 30, 0, NULL, '2026-04-12 09:42:57'),
(430, 315, 'Writing-Speaking Stamina and Performance Strategy', '', 4, 35, 0, NULL, '2026-04-12 09:42:57'),
(2001, 2001, 'Listening Section 1: Fitness Club Enquiry', 'listening', 2, 8, 0, NULL, '2026-05-16 15:44:50'),
(2002, 2001, 'Listening Section 2: Museum Tour Guide', 'listening', 2, 8, 0, NULL, '2026-05-16 15:44:50'),
(2003, 2001, 'Listening Section 3: Research Project Discussion', 'listening', 3, 10, 0, NULL, '2026-05-16 15:44:50'),
(2004, 2001, 'Listening Section 4: Coral Reef Conservation', 'listening', 4, 10, 0, NULL, '2026-05-16 15:44:50'),
(2005, 2002, 'Reading Passage 1: Urban Green Spaces', 'reading', 2, 20, 0, NULL, '2026-05-16 15:44:50'),
(2006, 2002, 'Reading Passage 2: Sleep and Learning', 'reading', 3, 20, 0, NULL, '2026-05-16 15:44:50'),
(2007, 2002, 'Reading Passage 3: AI in Healthcare', 'reading', 4, 20, 0, NULL, '2026-05-16 15:44:50'),
(2008, 2003, 'Writing Task 1: Internet Usage Chart', 'writing', 3, 20, 0, NULL, '2026-05-16 15:44:50'),
(2009, 2003, 'Writing Task 2: Foreign Language Education', 'writing', 3, 40, 0, NULL, '2026-05-16 15:44:50'),
(2010, 2004, 'Speaking Part 1: Food and Cooking', 'speaking', 2, 5, 0, NULL, '2026-05-16 15:44:50'),
(2011, 2004, 'Speaking Part 2: Helping Someone', 'speaking', 3, 4, 0, NULL, '2026-05-16 15:44:50'),
(2012, 2004, 'Speaking Part 3: Volunteering', 'speaking', 4, 5, 0, NULL, '2026-05-16 15:44:50');

-- --------------------------------------------------------

--
-- Table structure for table `tb_lesson_contents`
--

CREATE TABLE `tb_lesson_contents` (
  `content_id` int(11) NOT NULL,
  `lesson_id` int(11) DEFAULT NULL,
  `content_type` enum('video','article','audio','exercise') DEFAULT NULL,
  `content_body` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_lesson_contents`
--

INSERT INTO `tb_lesson_contents` (`content_id`, `lesson_id`, `content_type`, `content_body`) VALUES
(1, 1, 'article', 'Read the passage and classify statements as True/False/Not Given.'),
(501, 401, 'exercise', 'Part 1 flow: predict form fields, listen for spelling cues, and re-check corrected numbers.'),
(502, 403, 'article', 'Map tasks: anchor points first, then directional verbs (turn left, opposite, adjacent).'),
(503, 405, 'exercise', 'Part 3 matching: track each speaker stance before final alignment.'),
(504, 407, 'article', 'Part 4 lecture: identify signposts such as first, however, in contrast, finally.'),
(505, 409, 'article', 'Reading timing: target 18-20 minutes per passage and reserve review buffer.'),
(506, 410, 'exercise', 'For TFNG, match statement claim with explicit line evidence and scope words.'),
(507, 411, 'exercise', 'Matching headings: focus on paragraph main function, not isolated keywords.'),
(508, 414, 'article', 'Summary completion relies on paraphrase recognition and grammar-fit constraints.'),
(509, 415, 'article', 'Task 1: introduction + overview + detail paragraphs; avoid unsupported causes.'),
(510, 419, 'article', 'Task 2: clear thesis, topic sentence, evidence, and counterpoint management.'),
(511, 423, 'exercise', 'Part 1 answers should be natural, specific, and around 2-4 sentences.'),
(512, 425, 'exercise', 'Part 2 planning: note prompts for who/what/when/why, then expand with examples.'),
(513, 427, 'article', 'Part 3: move from personal examples to societal implications and policy viewpoints.'),
(514, 429, 'article', 'Full-test strategy: keep strict section pacing and minimize blank answers.'),
(2001, 2001, 'audio', 'Listen to a phone call about joining a fitness club. Complete the form.'),
(2002, 2002, 'audio', 'Listen to a museum guide explaining the layout and facilities.'),
(2003, 2003, 'audio', 'Two students and a tutor discuss a research assignment.'),
(2004, 2004, 'audio', 'A lecture on threats to coral reefs and conservation strategies.'),
(2005, 2005, 'article', 'Read the passage about urban green spaces and answer questions.'),
(2006, 2006, 'article', 'Read the passage about sleep research and answer questions.'),
(2007, 2007, 'article', 'Read the passage about AI in healthcare and answer questions.'),
(2008, 2008, 'exercise', 'Describe the chart showing internet usage by age group.'),
(2009, 2009, 'exercise', 'Write an essay about when children should start learning languages.'),
(2010, 2010, 'exercise', 'Answer questions about food, cooking, and eating habits.'),
(2011, 2011, 'exercise', 'Describe a time when you helped someone who needed it.'),
(2012, 2012, 'exercise', 'Discuss volunteering, community service, and social responsibility.');

-- --------------------------------------------------------

--
-- Table structure for table `tb_listening_materials`
--

CREATE TABLE `tb_listening_materials` (
  `material_id` int(11) NOT NULL,
  `lesson_id` int(11) DEFAULT NULL,
  `material_type` varchar(50) NOT NULL DEFAULT 'dialogue',
  `title` varchar(255) NOT NULL,
  `transcript` longtext NOT NULL,
  `audio_url` varchar(255) DEFAULT NULL,
  `duration_seconds` int(11) DEFAULT NULL,
  `speaker_count` int(11) DEFAULT NULL,
  `topics` varchar(500) DEFAULT NULL,
  `notes_for_learner` text DEFAULT NULL,
  `difficulty_level` varchar(50) NOT NULL DEFAULT 'band_5_6',
  `source` varchar(255) DEFAULT NULL,
  `created_at` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_listening_materials`
--

INSERT INTO `tb_listening_materials` (`material_id`, `lesson_id`, `material_type`, `title`, `transcript`, `audio_url`, `duration_seconds`, `speaker_count`, `topics`, `notes_for_learner`, `difficulty_level`, `source`, `created_at`) VALUES
(1, 2, 'conversation', 'Library Opening Hours', 'The library is open until 10 PM on weekdays.', 'https://example.com/audio/listening1.mp3', 180, 2, 'education,library', 'Focus on numbers and times.', 'band_5_6', 'Cambridge-style', '2026-04-12 11:33:21'),
(1001, 401, 'conversation', 'Part 1 Booking Call: City Museum Weekend Program', 'Agent and caller confirm booking details, date changes, spelling and payment.', 'https://example.com/audio/ielts/listening_part1_booking_call.mp3', 360, 2, 'booking,services,time-and-date', 'Expect corrections and distractors around numbers and names.', 'band_5_6', 'Research-aligned IELTS style', '2026-04-12 16:42:58'),
(1002, 403, 'monologue', 'Part 2 Orientation Talk: Community Event Venue', 'A speaker explains site layout, route instructions, and facilities for visitors.', 'https://example.com/audio/ielts/listening_part2_orientation_talk.mp3', 390, 1, 'directions,map,public-information', 'Track map landmarks and directional prepositions carefully.', 'band_5_6', 'Research-aligned IELTS style', '2026-04-12 16:42:58'),
(1003, 405, 'discussion', 'Part 3 Seminar Discussion: Student Research Design', 'Three speakers discuss methodology choices, risks, and pilot adjustments.', 'https://example.com/audio/ielts/listening_part3_seminar_discussion.mp3', 420, 3, 'education,research,analysis', 'Map each opinion to the correct speaker before selecting options.', 'band_6_7', 'Research-aligned IELTS style', '2026-04-12 16:42:58'),
(1004, 407, 'lecture', 'Part 4 Academic Lecture: Urban Mobility and Policy', 'A lecture presents framework, evidence, limitations, and recommendations.', 'https://example.com/audio/ielts/listening_part4_academic_lecture.mp3', 450, 1, 'urban-planning,policy,statistics', 'Use signposting words to predict next answer location.', 'band_6_7', 'Research-aligned IELTS style', '2026-04-12 16:42:58'),
(2001, 2001, 'conversation', 'Fitness Club Membership Enquiry', 'Receptionist: Good morning, Riverside Fitness Club. How can I help?\nCaller: Hi, I\'d like to ask about membership options.\nReceptionist: Of course. We have three tiers: Bronze at forty-five pounds per month, Silver at sixty-five, and Gold at eighty-five.\nCaller: What does Gold include?\nReceptionist: Unlimited gym access, pool, all group classes, and a personal training session every month. Plus free parking.\nCaller: That sounds good. My name is Katherine Bromley. That\'s B-R-O-M-L-E-Y.\nReceptionist: And your date of birth?\nCaller: The fourteenth of March, nineteen ninety-two.\nReceptionist: Great. We\'re open weekdays six AM to ten PM, and weekends eight to six. Your induction is booked for next Tuesday at half past nine.', '/audio/cam20/T1S1.m4a', 300, 2, 'fitness,membership,booking', 'Listen for corrected numbers, spelling, and times.', 'band_5_6', 'Cambridge IELTS 20 Academic', '2026-05-16 22:45:00'),
(2002, 2002, 'monologue', 'City Museum Visitor Information', 'Welcome to the City Heritage Museum. Before we start the tour, let me explain the layout. You\'re currently in the Main Hall. Directly ahead is the Ancient History gallery. If you turn left past the gift shop, you\'ll find the Science Wing, which opened last September. The café is on the ground floor, next to the east entrance. Toilets are located behind the information desk. The special exhibition on marine archaeology is on the second floor, running until the end of November. Photography is permitted in all areas except the Manuscript Room. We ask that you keep your voices low in the Reading Room on the third floor.', '/audio/cam20/T1S2.m4a', 330, 1, 'museum,directions,facilities', 'Track spatial language and location references.', 'band_5_6', 'Cambridge IELTS 20 Academic', '2026-05-16 22:45:00'),
(2003, 2003, 'discussion', 'Research Assignment: Water Conservation Study', 'Tutor: So, how is your water conservation project coming along?\nSara: We\'ve finished the literature review. The main challenge is the survey design.\nMark: I think we should use a stratified sample rather than random, because the campus population is quite diverse.\nTutor: That\'s sensible. What about your sample size?\nSara: We\'re aiming for a hundred and fifty responses. Mark wanted two hundred but the deadline is in three weeks.\nMark: True. And we need to decide on the analysis software. I\'ve used SPSS before but Sara prefers Python.\nTutor: Either works. The key thing is consistency. Also, have you considered the ethical approval? You\'ll need to submit the form by Friday.\nSara: Yes, we\'ve drafted the consent form already.', '/audio/cam20/T1S3.m4a', 380, 3, 'research,education,methodology', 'Match opinions to speakers and follow the discussion flow.', 'band_6_7', 'Cambridge IELTS 20 Academic', '2026-05-16 22:45:00'),
(2004, 2004, 'lecture', 'Threats to Coral Reef Ecosystems', 'Today I want to discuss the decline of coral reef ecosystems worldwide. Coral reefs support approximately twenty-five percent of all marine species despite covering less than one percent of the ocean floor. The primary threat is ocean warming, which causes coral bleaching. When water temperatures rise by just one to two degrees Celsius above the summer maximum, corals expel their symbiotic algae and turn white. If conditions persist beyond six weeks, mortality rates can exceed seventy percent. A second major factor is ocean acidification, caused by increased CO2 absorption. The pH of surface oceans has dropped by 0.1 units since pre-industrial times. Additionally, agricultural runoff introduces excess nutrients that promote algal blooms, blocking sunlight. Conservation strategies include establishing marine protected areas, reducing land-based pollution, and coral nursery programs where fragments are grown on underwater frames before transplanting to damaged reefs.', '/audio/cam20/T1S4.m4a', 420, 1, 'environment,marine-biology,conservation', 'Note signpost words and numerical data carefully.', 'band_6_7', 'Cambridge IELTS 20 Academic', '2026-05-16 22:45:00');

-- --------------------------------------------------------

--
-- Table structure for table `tb_listening_questions`
--

CREATE TABLE `tb_listening_questions` (
  `question_id` int(11) NOT NULL,
  `material_id` int(11) NOT NULL,
  `question_type` varchar(50) NOT NULL DEFAULT 'fill_in_blank',
  `question_text` text NOT NULL,
  `time_code_start` int(11) DEFAULT NULL,
  `time_code_end` int(11) DEFAULT NULL,
  `correct_answer` text NOT NULL,
  `options` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`options`)),
  `explanation` longtext DEFAULT NULL,
  `band_target` decimal(2,1) DEFAULT NULL,
  `question_order` int(11) NOT NULL,
  `created_at` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_listening_questions`
--

INSERT INTO `tb_listening_questions` (`question_id`, `material_id`, `question_type`, `question_text`, `time_code_start`, `time_code_end`, `correct_answer`, `options`, `explanation`, `band_target`, `question_order`, `created_at`) VALUES
(1, 1, 'multiple_choice', 'What time does the library close? ', 10, 25, '10 PM', '[\"8 PM\",\"9 PM\",\"10 PM\",\"11 PM\"]', 'Speaker clearly says ten PM.', 6.0, 1, '2026-04-12 11:33:21'),
(1101, 1001, 'form_completion', 'Booking reference is ____.', 12, 20, 'MZ-492', NULL, 'Alpha-numeric code confirmed after repetition.', 6.0, 1, '2026-04-12 16:42:58'),
(1102, 1001, 'form_completion', 'The session starts at ____.', 24, 33, '9:30', NULL, 'Caller corrects from 9:00 to 9:30.', 6.0, 2, '2026-04-12 16:42:58'),
(1103, 1001, 'form_completion', 'Total participants: ____.', 35, 43, '3', NULL, 'A child is added after first confirmation.', 6.0, 3, '2026-04-12 16:42:58'),
(1104, 1001, 'multiple_choice', 'Preferred payment method is:', 45, 55, 'C', '[\"A. cash\",\"B. transfer\",\"C. card\",\"D. voucher\"]', 'Card selected at final confirmation.', 6.0, 4, '2026-04-12 16:42:58'),
(1105, 1001, 'form_completion', 'Street name spelling: ____.', 58, 67, 'Hawthorne', NULL, 'Spelled letter-by-letter by caller.', 6.5, 5, '2026-04-12 16:42:58'),
(1106, 1001, 'short_answer', 'Arrival should be at least ____ early.', 70, 79, '15 minutes', NULL, 'Agent advises fifteen-minute buffer.', 6.0, 6, '2026-04-12 16:42:58'),
(1107, 1001, 'note_completion', 'Collection point is the ____ entrance.', 82, 92, 'east gate', NULL, 'Agent specifies east-side entry.', 6.0, 7, '2026-04-12 16:42:58'),
(1108, 1001, 'multiple_choice', 'The free add-on service is:', 95, 104, 'B', '[\"A. guidebook\",\"B. locker\",\"C. lunch\",\"D. transport\"]', 'Locker service included in package.', 6.5, 8, '2026-04-12 16:42:58'),
(1109, 1001, 'form_completion', 'Confirmation email domain ends with ____.', 107, 118, '.org', NULL, 'Domain suffix dictated clearly.', 6.0, 9, '2026-04-12 16:42:58'),
(1110, 1001, 'short_answer', 'Final booking month is ____.', 121, 132, 'November', NULL, 'Month repeated in closing summary.', 6.0, 10, '2026-04-12 16:42:58'),
(1111, 1002, 'map_labelling', 'On the map, point A is the ____ desk.', 15, 28, 'information', NULL, 'Speaker introduces main orientation point.', 6.0, 11, '2026-04-12 16:42:58'),
(1112, 1002, 'map_labelling', 'The first aid room is next to the ____.', 30, 44, 'hall', NULL, 'Location relation marked using adjacency.', 6.0, 12, '2026-04-12 16:42:58'),
(1113, 1002, 'map_labelling', 'Visitors should enter via the ____ gate.', 46, 56, 'north', NULL, 'Directional cue is explicit.', 6.0, 13, '2026-04-12 16:42:58'),
(1114, 1002, 'multiple_choice', 'Parking is restricted during:', 58, 70, 'D', '[\"A. weekdays\",\"B. mornings\",\"C. festivals\",\"D. peak events\"]', 'Restriction tied to event traffic.', 6.5, 14, '2026-04-12 16:42:58'),
(1115, 1002, 'table_completion', 'Maximum group size: ____.', 72, 84, '20', NULL, 'Numeric limit listed in policy notice.', 6.0, 15, '2026-04-12 16:42:58'),
(1116, 1002, 'note_completion', 'Audio guide languages include English and ____.', 86, 97, 'Spanish', NULL, 'Second language stated in amenities list.', 6.0, 16, '2026-04-12 16:42:58'),
(1117, 1002, 'flow_chart_completion', 'After registration, visitors proceed to ____.', 99, 113, 'security check', NULL, 'Step sequence highlighted with signpost.', 6.5, 17, '2026-04-12 16:42:58'),
(1118, 1002, 'multiple_choice', 'The quiet zone is located:', 115, 126, 'A', '[\"A. beside the archive room\",\"B. behind the café\",\"C. opposite ticket office\",\"D. above lecture hall\"]', 'Location described using contrast marker.', 6.5, 18, '2026-04-12 16:42:58'),
(1119, 1002, 'short_answer', 'Recommended route color is ____.', 128, 139, 'blue', NULL, 'Route line color repeated twice.', 6.0, 19, '2026-04-12 16:42:58'),
(1120, 1002, 'summary_completion', 'Main safety message: avoid ____ corridors.', 141, 154, 'restricted', NULL, 'Restriction term used in final reminder.', 6.5, 20, '2026-04-12 16:42:58'),
(1121, 1003, 'matching', 'Speaker 1 concern category.', 18, 34, 'methodology', NULL, 'Speaker 1 focuses on sampling approach.', 6.5, 21, '2026-04-12 16:42:58'),
(1122, 1003, 'matching', 'Speaker 2 concern category.', 36, 52, 'timeline', NULL, 'Speaker 2 emphasizes schedule pressure.', 6.5, 22, '2026-04-12 16:42:58'),
(1123, 1003, 'matching', 'Speaker 3 concern category.', 54, 68, 'budget', NULL, 'Speaker 3 highlights resource limits.', 6.5, 23, '2026-04-12 16:42:58'),
(1124, 1003, 'multiple_choice', 'Pilot method preferred by group:', 70, 86, 'B', '[\"A. random trial\",\"B. stratified pilot\",\"C. online survey only\",\"D. retrospective review\"]', 'Consensus formed after comparison.', 6.5, 24, '2026-04-12 16:42:58'),
(1125, 1003, 'multiple_choice', 'Risk considered most critical is:', 88, 103, 'C', '[\"A. legal delay\",\"B. staff shortage\",\"C. response bias\",\"D. equipment failure\"]', 'Response bias repeated in conclusion.', 6.5, 25, '2026-04-12 16:42:58'),
(1126, 1003, 'note_completion', 'Data cleaning stage follows ____.', 105, 119, 'collection', NULL, 'Order stated in workflow recap.', 6.5, 26, '2026-04-12 16:42:58'),
(1127, 1003, 'note_completion', 'Final proposal due in ____.', 121, 136, 'June', NULL, 'Month specified after negotiation.', 6.5, 27, '2026-04-12 16:42:58'),
(1128, 1003, 'summary_completion', 'External advisor requested ____ metrics.', 138, 152, 'comparative', NULL, 'Advisor asks for comparative indicators.', 7.0, 28, '2026-04-12 16:42:58'),
(1129, 1003, 'multiple_choice', 'Reason for rejecting option D:', 154, 168, 'A', '[\"A. cost\",\"B. ethics\",\"C. timing\",\"D. validity\"]', 'High cost cited as blocker.', 6.5, 29, '2026-04-12 16:42:58'),
(1130, 1003, 'short_answer', 'Meeting frequency agreed: ____.', 170, 184, 'monthly', NULL, 'Monthly cadence stated in closing.', 6.5, 30, '2026-04-12 16:42:58'),
(1131, 1004, 'note_completion', 'Lecture theme: urban ____ transition.', 14, 28, 'mobility', NULL, 'Topic defined in opening line.', 7.0, 31, '2026-04-12 16:42:58'),
(1132, 1004, 'note_completion', 'Primary indicator: trip ____.', 30, 45, 'efficiency', NULL, 'Indicator appears in framework definition.', 7.0, 32, '2026-04-12 16:42:58'),
(1133, 1004, 'table_completion', 'Reported modal-shift value: ____ percent.', 47, 60, '27', NULL, 'Statistic read once then repeated.', 7.0, 33, '2026-04-12 16:42:58'),
(1134, 1004, 'summary_completion', 'Variation cause: policy ____ across districts.', 62, 78, 'inconsistency', NULL, 'Cause linked to comparative districts.', 7.0, 34, '2026-04-12 16:42:58'),
(1135, 1004, 'flow_chart_completion', 'After normalization, run ____ modeling.', 80, 95, 'scenario', NULL, 'Step order shown with next marker.', 7.0, 35, '2026-04-12 16:42:58'),
(1136, 1004, 'summary_completion', 'Short-term recommendation: ____ lanes.', 97, 112, 'bus-priority', NULL, 'Intervention emphasized in policy block.', 7.0, 36, '2026-04-12 16:42:58'),
(1137, 1004, 'summary_completion', 'Long-term recommendation: integrated ____.', 114, 130, 'planning', NULL, 'Long-horizon reform highlighted.', 7.0, 37, '2026-04-12 16:42:58'),
(1138, 1004, 'short_answer', 'Minimum sample threshold: ____.', 132, 146, '120', NULL, 'Threshold stated in limitation section.', 7.0, 38, '2026-04-12 16:42:58'),
(1139, 1004, 'short_answer', 'Pilot duration: ____ weeks.', 148, 163, '12', NULL, 'Duration specified in implementation plan.', 7.0, 39, '2026-04-12 16:42:58'),
(1140, 1004, 'short_answer', 'Final reporting month: ____.', 165, 180, 'November', NULL, 'Month repeated in final slide summary.', 7.0, 40, '2026-04-12 16:42:58'),
(2001, 2001, 'form_completion', 'Monthly cost of Gold membership: £____', 10, 25, '85', NULL, 'Receptionist says eighty-five pounds.', 5.5, 1, '2026-05-16 22:45:00'),
(2002, 2001, 'form_completion', 'Caller surname spelling: ____', 30, 45, 'Bromley', NULL, 'Spelled out B-R-O-M-L-E-Y.', 5.5, 2, '2026-05-16 22:45:00'),
(2003, 2001, 'form_completion', 'Date of birth: 14th ____ 1992', 48, 60, 'March', NULL, 'Stated as fourteenth of March.', 5.5, 3, '2026-05-16 22:45:00'),
(2004, 2001, 'form_completion', 'Weekend closing time: ____', 65, 78, '6 PM', NULL, 'Weekends eight to six.', 5.5, 4, '2026-05-16 22:45:00'),
(2005, 2001, 'form_completion', 'Induction time: ____', 80, 95, '9:30', NULL, 'Half past nine on Tuesday.', 6.0, 5, '2026-05-16 22:45:00'),
(2006, 2002, 'note_completion', 'The Science Wing opened in ____.', 15, 30, 'September', NULL, 'Opened last September.', 6.0, 6, '2026-05-16 22:45:00'),
(2007, 2002, 'map_labelling', 'The café is next to the ____ entrance.', 32, 48, 'east', NULL, 'Next to the east entrance.', 6.0, 7, '2026-05-16 22:45:00'),
(2008, 2002, 'note_completion', 'The marine archaeology exhibition ends in ____.', 50, 65, 'November', NULL, 'Running until end of November.', 6.0, 8, '2026-05-16 22:45:00'),
(2009, 2002, 'multiple_choice', 'Photography is NOT allowed in:', 68, 80, 'C', '[\"A. Ancient History gallery\",\"B. Science Wing\",\"C. Manuscript Room\",\"D. Main Hall\"]', 'All areas except the Manuscript Room.', 6.0, 9, '2026-05-16 22:45:00'),
(2010, 2002, 'note_completion', 'Visitors should keep quiet in the ____ Room.', 82, 95, 'Reading', NULL, 'Keep voices low in the Reading Room.', 5.5, 10, '2026-05-16 22:45:00'),
(2011, 2003, 'multiple_choice', 'What sampling method does Mark prefer?', 18, 35, 'B', '[\"A. Random\",\"B. Stratified\",\"C. Convenience\",\"D. Cluster\"]', 'Mark suggests stratified sample.', 6.5, 11, '2026-05-16 22:45:00'),
(2012, 2003, 'form_completion', 'Target number of survey responses: ____', 38, 50, '150', NULL, 'Aiming for a hundred and fifty.', 6.5, 12, '2026-05-16 22:45:00'),
(2013, 2003, 'form_completion', 'Project deadline is in ____ weeks.', 52, 64, '3', NULL, 'Deadline is in three weeks.', 6.5, 13, '2026-05-16 22:45:00'),
(2014, 2003, 'matching', 'Sara prefers using ____ for analysis.', 66, 78, 'Python', NULL, 'Sara prefers Python.', 6.5, 14, '2026-05-16 22:45:00'),
(2015, 2003, 'short_answer', 'Ethical approval form due by ____.', 80, 95, 'Friday', NULL, 'Submit the form by Friday.', 6.5, 15, '2026-05-16 22:45:00'),
(2016, 2004, 'note_completion', 'Coral reefs support ____% of marine species.', 14, 28, '25', NULL, 'Twenty-five percent stated.', 7.0, 16, '2026-05-16 22:45:00'),
(2017, 2004, 'summary_completion', 'Bleaching occurs when temperature rises ____ degrees above maximum.', 30, 48, '1 to 2', NULL, 'One to two degrees Celsius.', 7.0, 17, '2026-05-16 22:45:00'),
(2018, 2004, 'note_completion', 'Mortality can exceed ____% if bleaching persists beyond 6 weeks.', 50, 65, '70', NULL, 'Seventy percent mortality rate.', 7.0, 18, '2026-05-16 22:45:00'),
(2019, 2004, 'summary_completion', 'Ocean pH has dropped by ____ units since pre-industrial times.', 68, 82, '0.1', NULL, 'pH drop of 0.1 units.', 7.0, 19, '2026-05-16 22:45:00'),
(2020, 2004, 'note_completion', 'In coral nurseries, fragments are grown on underwater ____.', 85, 100, 'frames', NULL, 'Grown on underwater frames.', 7.0, 20, '2026-05-16 22:45:00');

-- --------------------------------------------------------

--
-- Table structure for table `tb_messages`
--

CREATE TABLE `tb_messages` (
  `message_id` int(11) NOT NULL,
  `conversation_id` int(11) NOT NULL,
  `sender_id` int(11) NOT NULL,
  `content` text NOT NULL,
  `is_read` tinyint(1) NOT NULL DEFAULT 0,
  `created_at` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tb_modules`
--

CREATE TABLE `tb_modules` (
  `module_id` int(11) NOT NULL,
  `course_id` int(11) DEFAULT NULL,
  `title` varchar(255) DEFAULT NULL,
  `order_index` int(11) DEFAULT NULL,
  `is_deleted` tinyint(1) DEFAULT 0,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_modules`
--

INSERT INTO `tb_modules` (`module_id`, `course_id`, `title`, `order_index`, `is_deleted`, `deleted_at`, `updated_at`) VALUES
(1, 1, 'True/False/Not Given', 1, 0, NULL, '2026-04-12 04:33:20'),
(2, 2, 'Section 1-2 Practice', 1, 0, NULL, '2026-04-12 04:33:20'),
(3, 3, 'Task 1 Visual Description', 1, 0, NULL, '2026-04-12 04:33:20'),
(4, 4, 'Part 1 Interview', 1, 0, NULL, '2026-04-12 04:33:20'),
(301, 201, 'Listening Part 1: Everyday Social Transactions', 1, 0, NULL, '2026-04-12 09:42:57'),
(302, 201, 'Listening Part 2: Social Monologue and Directions', 2, 0, NULL, '2026-04-12 09:42:57'),
(303, 201, 'Listening Part 3: Academic Discussion', 3, 0, NULL, '2026-04-12 09:42:57'),
(304, 201, 'Listening Part 4: Academic Lecture', 4, 0, NULL, '2026-04-12 09:42:57'),
(305, 202, 'Reading Passage 1 Strategy and Warm-up Types', 1, 0, NULL, '2026-04-12 09:42:57'),
(306, 202, 'Reading Passage 2 Mid-level Inference and Matching', 2, 0, NULL, '2026-04-12 09:42:57'),
(307, 202, 'Reading Passage 3 High-density Academic Detail', 3, 0, NULL, '2026-04-12 09:42:57'),
(308, 203, 'Writing Task 1: Visual Data Reports', 1, 0, NULL, '2026-04-12 09:42:57'),
(309, 203, 'Writing Task 1: Process and Map Reports', 2, 0, NULL, '2026-04-12 09:42:57'),
(310, 203, 'Writing Task 2: Opinion and Discussion Essays', 3, 0, NULL, '2026-04-12 09:42:57'),
(311, 203, 'Writing Task 2: Problem-Solution and Adv/Disadv', 4, 0, NULL, '2026-04-12 09:42:57'),
(312, 204, 'Speaking Part 1: Personal Familiar Topics', 1, 0, NULL, '2026-04-12 09:42:57'),
(313, 204, 'Speaking Part 2: Cue Card Long Turn', 2, 0, NULL, '2026-04-12 09:42:57'),
(314, 204, 'Speaking Part 3: Two-way Abstract Discussion', 3, 0, NULL, '2026-04-12 09:42:57'),
(315, 205, 'Full Test Timing and Section Transfer Strategy', 1, 0, NULL, '2026-04-12 09:42:57'),
(2001, 2001, 'Listening - Test 1', 1, 0, NULL, '2026-05-16 15:44:50'),
(2002, 2001, 'Reading - Test 1', 2, 0, NULL, '2026-05-16 15:44:50'),
(2003, 2001, 'Writing - Test 1', 3, 0, NULL, '2026-05-16 15:44:50'),
(2004, 2001, 'Speaking - Test 1', 4, 0, NULL, '2026-05-16 15:44:50');

-- --------------------------------------------------------

--
-- Table structure for table `tb_notifications`
--

CREATE TABLE `tb_notifications` (
  `notification_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `type` enum('achievement','reminder','announcement','feedback','system') DEFAULT 'system',
  `title` varchar(255) NOT NULL,
  `message` text NOT NULL,
  `data` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`data`)),
  `is_read` tinyint(1) DEFAULT 0,
  `read_at` timestamp NULL DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `expires_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_notifications`
--

INSERT INTO `tb_notifications` (`notification_id`, `user_id`, `type`, `title`, `message`, `data`, `is_read`, `read_at`, `created_at`, `expires_at`) VALUES
(1, 1, 'system', 'Welcome to WebIeltsFree', 'Your learning roadmap is ready.', '{\"module\":\"onboarding\"}', 0, NULL, '2026-04-12 04:33:21', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `tb_questions`
--

CREATE TABLE `tb_questions` (
  `question_id` int(11) NOT NULL,
  `section_id` int(11) DEFAULT NULL,
  `question_text` text DEFAULT NULL,
  `difficulty` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_questions`
--

INSERT INTO `tb_questions` (`question_id`, `section_id`, `question_text`, `difficulty`) VALUES
(1, 1, 'Statement 1: Urban planning has changed due to climate risks.', 2),
(2, 2, 'What time does the library close on weekdays?', 2),
(8001, 701, 'Q1. Booking reference code for the museum tour form', 3),
(8002, 701, 'Q2. Start time of the Saturday orientation session', 3),
(8003, 701, 'Q3. Number of participants confirmed after correction', 3),
(8004, 701, 'Q4. Payment method selected by the caller', 3),
(8005, 701, 'Q5. Correct street name spelling for registration', 3),
(8006, 701, 'Q6. Main reason for changing appointment date', 3),
(8007, 701, 'Q7. Which entrance should visitors use?', 3),
(8008, 701, 'Q8. Facility available near Point C on the map', 3),
(8009, 701, 'Q9. Label for the information desk location', 3),
(8010, 701, 'Q10. Recommended arrival buffer before the event', 3),
(8011, 701, 'Q11. Speaker A view on sample size reliability', 3),
(8012, 701, 'Q12. Speaker B concern about data collection timeline', 3),
(8013, 701, 'Q13. Most suitable method proposed for pilot testing', 3),
(8014, 701, 'Q14. Reason for rejecting Option D in the seminar', 3),
(8015, 701, 'Q15. Matching: statement linked to Speaker 1', 3),
(8016, 701, 'Q16. Matching: statement linked to Speaker 2', 3),
(8017, 701, 'Q17. Matching: statement linked to Speaker 3', 3),
(8018, 701, 'Q18. Term used for secondary data source', 3),
(8019, 701, 'Q19. Key risk identified in methodology review', 3),
(8020, 701, 'Q20. Final decision before next meeting', 3),
(8021, 701, 'Q21. Lecture topic focus in first segment', 4),
(8022, 701, 'Q22. Definition used for sustainable transport index', 4),
(8023, 701, 'Q23. Percentage figure mentioned for mode shift', 4),
(8024, 701, 'Q24. Cause linked to variation in district outcomes', 4),
(8025, 701, 'Q25. Sequence step after baseline data cleaning', 4),
(8026, 701, 'Q26. One-word completion: policy lever type', 4),
(8027, 701, 'Q27. One-word completion: implementation barrier', 4),
(8028, 701, 'Q28. One-word completion: stakeholder group', 4),
(8029, 701, 'Q29. One-word completion: funding source', 4),
(8030, 701, 'Q30. One-word completion: evaluation metric', 4),
(8031, 701, 'Q31. Summary completion: year with peak growth', 4),
(8032, 701, 'Q32. Summary completion: city with lowest adoption', 4),
(8033, 701, 'Q33. Summary completion: principal confounding factor', 4),
(8034, 701, 'Q34. Summary completion: short-term intervention', 4),
(8035, 701, 'Q35. Summary completion: long-term recommendation', 4),
(8036, 701, 'Q36. Short answer: minimum sample threshold', 4),
(8037, 701, 'Q37. Short answer: meeting frequency proposed', 4),
(8038, 701, 'Q38. Short answer: data retention period', 4),
(8039, 701, 'Q39. Short answer: pilot duration', 4),
(8040, 701, 'Q40. Short answer: final reporting month', 4),
(8041, 702, 'Q1. Passage 1 TFNG: consolidation hubs always reduce costs for all businesses', 3),
(8042, 702, 'Q2. Passage 1 TFNG: policy support varies by region', 3),
(8043, 702, 'Q3. Passage 1 MCQ: main purpose of Paragraph C', 3),
(8044, 702, 'Q4. Passage 1 matching heading for Paragraph A', 3),
(8045, 702, 'Q5. Passage 1 sentence completion with max two words', 3),
(8046, 702, 'Q6. Passage 1 summary completion item 1', 3),
(8047, 702, 'Q7. Passage 1 summary completion item 2', 3),
(8048, 702, 'Q8. Passage 1 matching information to paragraph', 3),
(8049, 702, 'Q9. Passage 1 short answer (one word)', 3),
(8050, 702, 'Q10. Passage 1 short answer (number)', 3),
(8051, 702, 'Q11. Passage 2 YNNG: writer supports fully digital-only libraries', 3),
(8052, 702, 'Q12. Passage 2 MCQ: target group of literacy program', 3),
(8053, 702, 'Q13. Passage 2 matching heading Paragraph B', 3),
(8054, 702, 'Q14. Passage 2 matching feature: funding model', 3),
(8055, 702, 'Q15. Passage 2 sentence completion 1', 3),
(8056, 702, 'Q16. Passage 2 sentence completion 2', 3),
(8057, 702, 'Q17. Passage 2 summary completion 1', 3),
(8058, 702, 'Q18. Passage 2 summary completion 2', 3),
(8059, 702, 'Q19. Passage 2 table completion item', 3),
(8060, 702, 'Q20. Passage 2 short answer question', 3),
(8061, 702, 'Q21. Passage 3 TFNG item 1', 4),
(8062, 702, 'Q22. Passage 3 TFNG item 2', 4),
(8063, 702, 'Q23. Passage 3 MCQ inference item 1', 4),
(8064, 702, 'Q24. Passage 3 MCQ inference item 2', 4),
(8065, 702, 'Q25. Passage 3 matching heading Paragraph D', 4),
(8066, 702, 'Q26. Passage 3 matching information paragraph reference', 4),
(8067, 702, 'Q27. Passage 3 sentence completion 1', 4),
(8068, 702, 'Q28. Passage 3 sentence completion 2', 4),
(8069, 702, 'Q29. Passage 3 summary completion 1', 4),
(8070, 702, 'Q30. Passage 3 summary completion 2', 4),
(8071, 702, 'Q31. Passage 3 summary completion 3', 4),
(8072, 702, 'Q32. Passage 3 note completion 1', 4),
(8073, 702, 'Q33. Passage 3 note completion 2', 4),
(8074, 702, 'Q34. Passage 3 table completion 1', 4),
(8075, 702, 'Q35. Passage 3 table completion 2', 4),
(8076, 702, 'Q36. Passage 3 diagram label item', 4),
(8077, 702, 'Q37. Passage 3 short answer 1', 4),
(8078, 702, 'Q38. Passage 3 short answer 2', 4),
(8079, 702, 'Q39. Passage 3 short answer 3', 4),
(8080, 702, 'Q40. Passage 3 short answer 4', 4),
(8081, 703, 'Task 1: Summarise data from a bar chart showing household spending categories across three cities (2010 vs 2020).', 3),
(8082, 703, 'Task 2: Discuss both views on remote work versus office-based work and give your opinion.', 3),
(8083, 704, 'Part 1: Hometown and routine questions', 3),
(8084, 704, 'Part 2: Describe a useful skill you learned recently', 3),
(8085, 704, 'Part 3: Discuss how technology changes social interaction', 3),
(8088, 2011, 'a', 6);

-- --------------------------------------------------------

--
-- Table structure for table `tb_reading_passages`
--

CREATE TABLE `tb_reading_passages` (
  `passage_id` int(11) NOT NULL,
  `lesson_id` int(11) DEFAULT NULL,
  `passage_title` varchar(255) NOT NULL,
  `passage_text` longtext NOT NULL,
  `word_count` int(11) DEFAULT NULL,
  `difficulty_level` varchar(50) NOT NULL DEFAULT 'band_5_6',
  `topic_category` varchar(100) DEFAULT NULL,
  `source` varchar(255) DEFAULT NULL,
  `created_at` datetime DEFAULT current_timestamp(),
  `image_url` varchar(255) DEFAULT NULL,
  `passage_translation` longtext DEFAULT NULL,
  `vocab_highlights` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_reading_passages`
--

INSERT INTO `tb_reading_passages` (`passage_id`, `lesson_id`, `passage_title`, `passage_text`, `word_count`, `difficulty_level`, `topic_category`, `source`, `created_at`, `image_url`, `passage_translation`, `vocab_highlights`) VALUES
(1, 1, 'Urban Climate Planning', 'Paragraph A: Cities are adapting to climate change through resilient planning...', 120, 'band_6_7', 'academic', 'Cambridge-style', '2026-04-12 11:33:21', NULL, 'Paragraph A: Các thành phố đang thích nghi với biến đổi khí hậu thông qua quy hoạch kiên cường...', '[\n    {\n      \"word\": \"adapting\",\n      \"ipa\": \"/əˈdæptɪŋ/\",\n      \"vi\": \"thích nghi, điều chỉnh\",\n      \"en\": \"adjusting to new conditions\"\n    },\n    {\n      \"word\": \"climate change\",\n      \"ipa\": \"/ˈklaɪmət tʃeɪndʒ/\",\n      \"vi\": \"biến đổi khí hậu\",\n      \"en\": \"a significant and lasting change in the weather patterns over periods ranging from decades to millions of years\"\n    },\n    {\n      \"word\": \"resilient\",\n      \"ipa\": \"/rɪˈzɪliənt/\",\n      \"vi\": \"kiên cường, có khả năng phục hồi\",\n      \"en\": \"able to withstand or recover quickly from difficult conditions\"\n    },\n    {\n      \"word\": \"planning\",\n      \"ipa\": \"/ˈplænɪŋ/\",\n      \"vi\": \"quy hoạch, lập kế hoạch\",\n      \"en\": \"the process of making plans for something\"\n    }\n  ]'),
(1201, 410, 'Urban Logistics and Last-mile Delivery', 'Paragraph A: Rapid urbanisation changes how goods enter dense city cores. Paragraph B: Consolidation hubs can reduce duplicate routes and curb congestion under suitable policy frameworks. Paragraph C: Adoption outcomes vary because implementation costs, zoning constraints, and coordination quality differ by region. Paragraph D: Long-term improvements depend on integrated planning and transparent performance metrics.', 258, 'band_5_6', 'urban policy', 'Research-aligned IELTS style', '2026-04-12 16:42:58', NULL, NULL, NULL),
(1202, 411, 'Community Libraries in the Digital Era', 'Paragraph A: Libraries increasingly combine traditional lending with digital-skills support. Paragraph B: Programs often target older adults and newly arrived residents who require practical online access. Paragraph C: Funding remains uneven and can shape staffing, opening hours, and continuity of services. Paragraph D: Hybrid service models may improve inclusiveness when designed around local needs.', 246, 'band_5_6', 'education', 'Research-aligned IELTS style', '2026-04-12 16:42:58', NULL, NULL, NULL),
(1203, 413, 'Adaptive City Planning Under Climate Pressure', 'Paragraph A: Urban planners balance short-term mobility demands with long-term resilience goals. Paragraph B: Data-driven systems improve intervention precision but raise governance and trust concerns. Paragraph C: Pilot schemes often show promising early outcomes, yet scaling requires stable financing and inter-agency alignment. Paragraph D: Effective policy cycles combine baseline measurement, staged rollouts, and periodic review.', 271, 'band_6_7', 'environment', 'Research-aligned IELTS style', '2026-04-12 16:42:58', NULL, NULL, NULL),
(2001, 2005, 'Urban Green Spaces and Public Health', 'Paragraph A: In recent decades, city planners have recognised that urban green spaces — parks, gardens, tree-lined streets and riverbanks — play a vital role in improving the physical and mental health of residents. Studies conducted in cities across Europe and North America consistently show that people living within 300 metres of a park report lower levels of stress, anxiety and cardiovascular disease.\n\nParagraph B: The mechanisms behind these benefits are both direct and indirect. Direct exposure to natural environments has been shown to reduce cortisol levels and lower blood pressure within as little as twenty minutes. Indirectly, parks encourage physical activity: walking, cycling and outdoor exercise that might not otherwise occur in dense urban settings.\n\nParagraph C: Despite the evidence, many rapidly growing cities face a shortage of accessible green space. Land values in central districts often make park creation financially unviable, and existing green areas are sometimes sold for commercial development. In response, some municipalities have adopted innovative approaches such as rooftop gardens, vertical forests on building facades, and converting disused railway lines into linear parks.\n\nParagraph D: Critics point out that the distribution of green space is frequently unequal. Wealthier neighbourhoods tend to have more and better-maintained parks, while lower-income areas may have only small, poorly equipped lots. Addressing this disparity requires targeted investment policies that prioritise underserved communities rather than allocating funds evenly across all districts.', 310, 'band_5_6', 'health and environment', 'Cambridge IELTS 20 style', '2026-05-16 22:45:08', 'https://images.unsplash.com/photo-1542601906990-b4d3fb778b09?q=80&w=1200', 'Paragraph A: Trong những thập kỷ gần đây, các nhà quy hoạch đô thị đã nhận ra rằng các không gian xanh đô thị — công viên, khu vườn, những con đường rợp bóng cây và bờ sông — đóng vai trò quan trọng trong việc cải thiện sức khỏe thể chất và tinh thần của người dân. Các nghiên cứu được thực hiện tại các thành phố trên khắp châu Âu và Bắc Mỹ liên tục chỉ ra rằng những người sống trong vòng 300 mét từ công viên báo cáo mức độ căng thẳng, lo âu và bệnh tim mạch thấp hơn.\r\n\r\nParagraph B: Các cơ chế đằng sau những lợi ích này vừa trực tiếp vừa gián tiếp. Tiếp xúc trực tiếp với môi trường tự nhiên đã được chứng minh là làm giảm nồng độ cortisol và giảm huyết áp chỉ trong vòng hai mươi phút. Gián tiếp, các công viên khuyến khích hoạt động thể chất: đi bộ, đạp xe và tập thể dục ngoài trời mà có lẽ sẽ không xảy ra trong các môi trường đô thị đông đúc.\r\n\r\nParagraph C: Bất chấp các bằng chứng, nhiều thành phố phát triển nhanh chóng đang đối mặt với tình trạng thiếu không gian xanh dễ tiếp cận. Giá đất ở các khu vực trung tâm thường khiến việc tạo lập công viên không khả thi về mặt tài chính, và các khu vực xanh hiện tại đôi khi bị bán để phát triển thương mại. Để ứng phó, một số chính quyền đô thị đã áp dụng các phương pháp đổi mới như vườn trên sân thượng, rừng thẳng đứng trên mặt đứng tòa nhà, và chuyển đổi các tuyến đường sắt không sử dụng thành công viên tuyến tính.\r\n\r\nParagraph D: Các nhà phê bình chỉ ra rằng sự phân bổ không gian xanh thường không bình đẳng. Các khu dân cư giàu có hơn có xu hướng có nhiều công viên hơn và được bảo dưỡng tốt hơn, trong khi các khu vực có thu nhập thấp hơn chỉ có những khu đất nhỏ, được trang bị kém. Giải quyết sự chênh lệch này đòi hỏi các chính sách đầu tư có mục tiêu ưu tiên các cộng đồng chưa được phục vụ đầy đủ thay vì phân bổ kinh phí đồng đều trên tất cả các quận huyện.', '[\r\n  {\"word\": \"cardiovascular\", \"ipa\": \"/ˌkɑː.di.əʊˈvæs.kjə.lər/\", \"vi\": \"thuộc tim mạch\", \"en\": \"relating to the heart and blood vessels\"},\r\n  {\"word\": \"cortisol\", \"ipa\": \"/ˈkɔː.tɪ.zɒl/\", \"vi\": \"hoóc môn cortisol\", \"en\": \"a steroid hormone produced by the adrenal glands in response to stress\"},\r\n  {\"word\": \"unviable\", \"ipa\": \"/ʌnˈvaɪ.ə.bəl/\", \"vi\": \"không khả thi\", \"en\": \"not capable of working successfully; not financially feasible\"},\r\n  {\"word\": \"disparity\", \"ipa\": \"/dɪˈspær.ə.ti/\", \"vi\": \"sự chênh lệch, bất bình đẳng\", \"en\": \"a great difference, especially one connected with unfair treatment\"},\r\n  {\"word\": \"municipalities\", \"ipa\": \"/mjuːˌnɪs.ɪˈpæl.ə.tiz/\", \"vi\": \"chính quyền thành phố\", \"en\": \"towns or districts that have local government\"},\r\n  {\"word\": \"linear\", \"ipa\": \"/ˈlɪn.i.ər/\", \"vi\": \"tuyến tính, kéo dài\", \"en\": \"arranged in or extending along a straight line\"},\r\n  {\"word\": \"underserved\", \"ipa\": \"/ˌʌn.dəˈsɜːvd/\", \"vi\": \"chưa được hỗ trợ đầy đủ\", \"en\": \"provided with inadequate services or facilities\"}\r\n]'),
(2002, 2006, 'Sleep, Memory and Academic Performance', 'Paragraph A: The relationship between sleep and cognitive function has been a subject of scientific inquiry for over a century. Modern neuroscience has confirmed what students have long suspected: adequate sleep is essential for consolidating new information into long-term memory. During deep sleep stages, the hippocampus replays patterns of neural activity recorded during the day, transferring them to the neocortex for permanent storage.\n\nParagraph B: Research at the University of Lübeck demonstrated that participants who slept for eight hours after learning a set of word pairs recalled 92 percent of them correctly the following day, compared with just 74 percent for a group that remained awake for the same period. Crucially, the sleeping group also showed superior performance on creative problem-solving tasks, suggesting that sleep enhances not only rote memory but also flexible thinking.\n\nParagraph C: Despite this evidence, surveys indicate that university students average only 6.2 hours of sleep per night — well below the recommended seven to nine hours. The consequences extend beyond memory: sleep deprivation impairs attention, decision-making and emotional regulation. A longitudinal study tracking 3,000 undergraduates found that each hour of sleep lost per night was associated with a 0.15 drop in cumulative grade point average over an academic year.\n\nParagraph D: Universities have begun to respond. Several institutions now schedule no examinations before 10 AM, and some offer workshops on sleep hygiene as part of orientation programmes. However, cultural attitudes that glorify late-night studying remain a significant barrier to change.', 280, 'band_6_7', 'education and science', 'Cambridge IELTS 20 style', '2026-05-16 22:45:08', 'https://images.unsplash.com/photo-1511295742364-92767fc4a28f?q=80&w=1200', 'Paragraph A: Mối quan hệ giữa giấc ngủ và chức năng nhận thức đã là chủ đề của cuộc điều tra khoa học trong hơn một thế kỷ. Thần kinh học hiện đại đã xác nhận điều mà sinh viên từ lâu đã nghi ngờ: giấc ngủ đầy đủ là điều cần thiết để củng cố thông tin mới vào trí nhớ dài hạn. Trong các giai đoạn giấc ngủ sâu, hồi hải mã tái hiện các mô hình hoạt động thần kinh được ghi lại trong ngày, chuyển chúng đến vỏ não mới để lưu trữ vĩnh viễn.\r\n\r\nParagraph B: Nghiên cứu tại Đại học Lübeck đã chứng minh rằng những người tham gia ngủ trong tám giờ sau khi học một bộ cặp từ đã nhớ lại chính xác 92% trong số đó vào ngày hôm sau, so với chỉ 74% ở nhóm thức trong cùng một khoảng thời gian. Quan trọng là, nhóm ngủ cũng cho thấy hiệu suất vượt trội trong các nhiệm vụ giải quyết vấn đề sáng tạo, gợi ý rằng giấc ngủ không chỉ tăng cường trí nhớ học vẹt mà còn cả tư duy linh hoạt.\r\n\r\nParagraph C: Bất chấp bằng chứng này, các cuộc khảo sát chỉ ra rằng sinh viên đại học chỉ ngủ trung bình 6,2 giờ mỗi đêm — thấp hơn nhiều so với mức khuyến nghị từ bảy đến chín giờ. Hậu quả vượt ra ngoài trí nhớ: thiếu ngủ làm suy giảm sự chú ý, ra quyết định và điều chỉnh cảm xúc. Một nghiên cứu dọc theo dõi 3.000 sinh viên đại học phát hiện ra rằng mỗi giờ ngủ mất đi mỗi đêm có liên quan đến việc giảm 0,15 điểm trung bình tích lũy trong một năm học.\r\n\r\nParagraph D: Các trường đại học đã bắt đầu ứng phó. Một số tổ chức hiện không lên lịch thi cử trước 10 giờ sáng, và một số cung cấp các hội thảo về vệ sinh giấc ngủ như một phần của chương trình định hướng. Tuy nhiên, các thái độ văn hóa tôn vinh việc học bài đêm muộn vẫn là một rào cản đáng kể đối với sự thay đổi.', '[\r\n  {\"word\": \"cognitive\", \"ipa\": \"/ˈɒɡ.nə.tɪv/\", \"vi\": \"thuộc về nhận thức\", \"en\": \"relating to the mental action or process of acquiring knowledge\"},\r\n  {\"word\": \"consolidating\", \"ipa\": \"/kənˈsɒl.ɪ.deɪ.tɪŋ/\", \"vi\": \"củng cố, hợp nhất\", \"en\": \"reinforcing or strengthening a connection or memory\"},\r\n  {\"word\": \"hippocampus\", \"ipa\": \"/ˌhɪp.əˈkæm.pəs/\", \"vi\": \"hồi hải mã\", \"en\": \"a part of the brain involved in forming and consolidating memories\"},\r\n  {\"word\": \"neocortex\", \"ipa\": \"/ˌniː.əʊˈkɔː.teks/\", \"vi\": \"vỏ não mới\", \"en\": \"the part of the brain involved in higher-order brain functions\"},\r\n  {\"word\": \"rote\", \"ipa\": \"/rəʊt/\", \"vi\": \"học vẹt, học lòng\", \"en\": \"mechanical or habitual repetition of something to be learned\"},\r\n  {\"word\": \"deprivation\", \"ipa\": \"/ˌdep.rɪˈveɪ.ʃən/\", \"vi\": \"sự thiếu hụt, mất ngủ\", \"en\": \"the state of lacking basic necessities like sleep or food\"},\r\n  {\"word\": \"longitudinal\", \"ipa\": \"/ˌlɒŋ.ɡɪˈtʃuː.dɪ.nəl/\", \"vi\": \"theo chiều dọc, dài hạn\", \"en\": \"observing or tracking variables over an extended period of time\"}\r\n]'),
(2003, 2007, 'Artificial Intelligence in Clinical Diagnostics', 'Paragraph A: The application of artificial intelligence to medical diagnostics represents one of the most promising and contentious developments in contemporary healthcare. Machine-learning algorithms trained on millions of labelled medical images can now identify certain conditions — including diabetic retinopathy, skin cancer and pneumonia — with accuracy comparable to, and occasionally exceeding, that of experienced specialists.\n\nParagraph B: A 2019 meta-analysis published in The Lancet Digital Health reviewed 82 studies comparing deep-learning systems with healthcare professionals. The pooled sensitivity of AI systems was 87 percent, matching the clinicians\' 86.4 percent. However, the specificity of AI was lower in several categories, meaning it produced more false positives — flagging healthy patients as potentially ill.\n\nParagraph C: Regulatory frameworks have struggled to keep pace with the technology. Traditional medical device approval assumes a fixed product, yet AI models may be updated continuously as new data becomes available. The US Food and Drug Administration introduced a pilot programme in 2021 allowing iterative updates under a predetermined change-control plan, but many jurisdictions still lack equivalent guidance.\n\nParagraph D: Ethical concerns centre on data bias and accountability. If a diagnostic algorithm is trained predominantly on images from one ethnic group, its accuracy may decline for others. Furthermore, when an AI system contributes to a misdiagnosis, assigning legal responsibility between the developer, the hospital and the supervising clinician remains unresolved in most legal systems.', 270, 'band_6_7', 'technology and medicine', 'Cambridge IELTS 20 style', '2026-05-16 22:45:08', 'https://images.unsplash.com/photo-1576091160399-112ba8d25d1d?q=80&w=1200', 'Paragraph A: Việc áp dụng trí tuệ nhân tạo vào chẩn đoán lâm sàng đại diện cho một trong những sự phát triển đầy hứa hẹn và gây tranh cãi nhất trong chăm sóc sức khỏe đương đại. Các thuật toán học máy được huấn luyện trên hàng triệu hình ảnh y khoa được dán nhãn hiện có thể xác định một số tình trạng bệnh — bao gồm bệnh võng mạc tiểu đường, ung thư da và viêm phổi — với độ chính xác tương đương và đôi khi vượt qua cả các chuyên gia giàu kinh nghiệm.\r\n\r\nParagraph B: Một phân tích gộp năm 2019 được công bố trên The Lancet Digital Health đã đánh giá 82 nghiên cứu so sánh các hệ thống học sâu với các chuyên gia y tế. Độ nhạy gộp của các hệ thống AI là 87%, khớp với tỷ lệ 86,4% của các bác sĩ lâm sàng. Tuy nhiên, độ đặc hiệu của AI thấp hơn ở một số danh mục, nghĩa là nó tạo ra nhiều kết quả dương tính giả hơn — gắn cờ những bệnh nhân khỏe mạnh là có khả năng bị bệnh.\r\n\r\nParagraph C: Các khung pháp lý đã gặp khó khăn trong việc bắt kịp với công nghệ. Phê duyệt thiết bị y tế truyền thống giả định một sản phẩm cố định, tuy nhiên các mô hình AI có thể được cập nhật liên tục khi có dữ liệu mới. Cục Quản lý Thực phẩm và Dược phẩm Hoa Kỳ đã giới thiệu một chương trình thí điểm vào năm 2021 cho phép các cập nhật lặp lại theo kế hoạch kiểm soát thay đổi định trước, nhưng nhiều cơ quan tài phán vẫn thiếu hướng dẫn tương đương.\r\n\r\nParagraph D: Các mối quan tâm về đạo đức tập trung vào định kiến dữ liệu và trách nhiệm giải trình. Nếu một thuật toán chẩn đoán được huấn luyện chủ yếu trên hình ảnh từ một nhóm dân tộc, độ chính xác của nó có thể giảm đối với những nhóm khác. Hơn nữa, khi một hệ thống AI góp phần vào việc chẩn đoán sai, việc phân chia trách nhiệm pháp lý giữa nhà phát triển, bệnh viện và bác sĩ lâm sàng giám sát vẫn chưa được giải quyết trong hầu hết các hệ thống pháp luật.', '[\r\n  {\"word\": \"diagnostics\", \"ipa\": \"/ˌdaɪ.əɡˈnɒs.tɪks/\", \"vi\": \"chẩn đoán học\", \"en\": \"the practice or science of identifying the nature of an illness\"},\r\n  {\"word\": \"contentious\", \"ipa\": \"/kənˈten.ʃəs/\", \"vi\": \"gây tranh cãi\", \"en\": \"causing or likely to cause an argument; controversial\"},\r\n  {\"word\": \"retinopathy\", \"ipa\": \"/ˌret.ɪˈnɒp.ə.θi/\", \"vi\": \"bệnh võng mạc\", \"en\": \"disease of the retina which results in impairment or loss of vision\"},\r\n  {\"word\": \"sensitivity\", \"ipa\": \"/ˌsen.sɪˈtɪv.ə.ti/\", \"vi\": \"độ nhạy\", \"en\": \"the ability of a diagnostic test to correctly identify those with the disease\"},\r\n  {\"word\": \"specificity\", \"ipa\": \"/ˌspes.ɪˈfɪs.ə.ti/\", \"vi\": \"độ đặc hiệu\", \"en\": \"the ability of a diagnostic test to correctly identify those without the disease\"},\r\n  {\"word\": \"iterative\", \"ipa\": \"/ˈɪt.ər.ə.tɪv/\", \"vi\": \"lặp đi lặp lại\", \"en\": \"relating to or involving repetition, doing something repeatedly to improve it\"},\r\n  {\"word\": \"jurisdictions\", \"ipa\": \"/ˌdʒʊə.rɪsˈdɪk.ʃənz/\", \"vi\": \"khu vực tài phán\", \"en\": \"the official power to make legal decisions and judgments\"}\r\n]');

-- --------------------------------------------------------

--
-- Table structure for table `tb_reading_questions`
--

CREATE TABLE `tb_reading_questions` (
  `question_id` int(11) NOT NULL,
  `passage_id` int(11) NOT NULL,
  `question_type` varchar(50) NOT NULL DEFAULT 'multiple_choice',
  `question_number` int(11) NOT NULL,
  `question_text` text NOT NULL,
  `correct_answer` text NOT NULL,
  `options` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`options`)),
  `explanation` longtext DEFAULT NULL,
  `band_target` decimal(2,1) DEFAULT NULL,
  `paragraph_reference` varchar(10) DEFAULT NULL,
  `created_at` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_reading_questions`
--

INSERT INTO `tb_reading_questions` (`question_id`, `passage_id`, `question_type`, `question_number`, `question_text`, `correct_answer`, `options`, `explanation`, `band_target`, `paragraph_reference`, `created_at`) VALUES
(1, 1, 'true_false_not_given', 1, 'Urban planning now considers climate resilience.', 'True', '[\"True\",\"False\",\"Not Given\"]', 'Stated directly in passage.', 6.5, 'A', '2026-04-12 11:33:21'),
(1301, 1201, 'true_false_not_given', 1, 'The passage states that consolidation hubs always lower costs for every operator.', 'Not Given', '[\"True\",\"False\",\"Not Given\"]', 'The text mentions route reduction but does not guarantee universal cost reduction.', 6.0, 'B', '2026-04-12 16:42:58'),
(1302, 1201, 'true_false_not_given', 2, 'Regional policy differences affect implementation outcomes.', 'True', '[\"True\",\"False\",\"Not Given\"]', 'Paragraph C explicitly links outcomes to policy variation.', 6.0, 'C', '2026-04-12 16:42:58'),
(1303, 1201, 'multiple_choice', 3, 'What is the main focus of Paragraph C?', 'C', '[\"A. Historical freight volumes\",\"B. Consumer psychology\",\"C. Constraints on implementation\",\"D. Vehicle technology\"]', 'Paragraph C details barriers and variability.', 6.5, 'C', '2026-04-12 16:42:58'),
(1304, 1201, 'matching_heading', 4, 'Choose the best heading for Paragraph A.', 'ii', '[\"i. Funding reforms\",\"ii. Changing urban freight demand\",\"iii. Case-study limitations\",\"iv. Digital literacy benefits\"]', 'Paragraph A introduces demand changes from urbanisation.', 6.0, 'A', '2026-04-12 16:42:58'),
(1305, 1201, 'sentence_completion', 5, 'Consolidation hubs can reduce duplicate ____ in city centers.', 'routes', NULL, 'Core mechanism described in Paragraph B.', 6.0, 'B', '2026-04-12 16:42:58'),
(1306, 1201, 'summary_completion', 6, 'One barrier to adoption is high implementation ____.', 'costs', NULL, 'Named directly in Paragraph C.', 6.0, 'C', '2026-04-12 16:42:58'),
(1307, 1201, 'summary_completion', 7, 'Long-term progress depends on integrated ____.', 'planning', NULL, 'Paragraph D states integrated planning is required.', 6.0, 'D', '2026-04-12 16:42:58'),
(1308, 1201, 'sentence_completion', 8, 'Performance should be tracked with transparent ____.', 'metrics', NULL, 'Paragraph D references transparent metrics.', 6.5, 'D', '2026-04-12 16:42:58'),
(1309, 1201, 'multiple_choice', 9, 'Which statement best reflects the author view?', 'B', '[\"A. Policy has little effect\",\"B. Context shapes effectiveness\",\"C. Technology alone is sufficient\",\"D. Short pilots prove long-term success\"]', 'Author emphasizes contextual variation.', 6.5, 'C', '2026-04-12 16:42:58'),
(1310, 1201, 'sentence_completion', 10, 'City-core congestion can worsen when routes are ____.', 'duplicated', NULL, 'Derived from Paragraph B mechanism.', 6.0, 'B', '2026-04-12 16:42:58'),
(1311, 1201, 'summary_completion', 11, 'Outcome differences are linked to zoning ____.', 'constraints', NULL, 'Paragraph C mentions zoning constraints.', 6.5, 'C', '2026-04-12 16:42:58'),
(1312, 1201, 'multiple_choice', 12, 'Which section mentions transparent performance tracking?', 'D', '[\"A\",\"B\",\"C\",\"D\"]', 'Mentioned in Paragraph D.', 6.0, 'D', '2026-04-12 16:42:58'),
(1313, 1201, 'sentence_completion', 13, 'A coordinated policy framework supports sustained ____.', 'improvement', NULL, 'Overall implication from Paragraph D.', 6.5, 'D', '2026-04-12 16:42:58'),
(1314, 1202, 'yes_no_not_given', 14, 'The writer argues libraries should be fully digital-only.', 'No', '[\"Yes\",\"No\",\"Not Given\"]', 'Passage supports hybrid model, not fully digital.', 6.0, 'D', '2026-04-12 16:42:58'),
(1315, 1202, 'multiple_choice', 15, 'Who is explicitly mentioned as a key target group?', 'A', '[\"A. Older adults\",\"B. School principals\",\"C. Tourists\",\"D. Investors\"]', 'Paragraph B highlights older adults.', 6.0, 'B', '2026-04-12 16:42:58'),
(1316, 1202, 'matching_heading', 16, 'Best heading for Paragraph B.', 'iii', '[\"i. Architecture trends\",\"ii. National legal reform\",\"iii. Digital inclusion support\",\"iv. Book-sales growth\"]', 'Paragraph B focuses on inclusion through digital skills.', 6.0, 'B', '2026-04-12 16:42:58'),
(1317, 1202, 'sentence_completion', 17, 'Funding disparities influence staffing and opening ____.', 'hours', NULL, 'Paragraph C includes opening hours impact.', 6.0, 'C', '2026-04-12 16:42:58'),
(1318, 1202, 'summary_completion', 18, 'Hybrid models may improve ____ in local communities.', 'inclusiveness', NULL, 'Paragraph D states inclusiveness benefit.', 6.0, 'D', '2026-04-12 16:42:58'),
(1319, 1202, 'multiple_choice', 19, 'Main concern raised in Paragraph C is:', 'B', '[\"A. Lack of books\",\"B. Uneven funding\",\"C. Staff training quality\",\"D. Outdated furniture\"]', 'Paragraph C centers on funding unevenness.', 6.5, 'C', '2026-04-12 16:42:58'),
(1320, 1202, 'sentence_completion', 20, 'Libraries now combine lending with ____ support.', 'digital-skills', NULL, 'Paragraph A phrasing.', 6.0, 'A', '2026-04-12 16:42:58'),
(1321, 1202, 'summary_completion', 21, 'Service continuity can depend on available ____.', 'funding', NULL, 'Paragraph C directly links continuity and funding.', 6.0, 'C', '2026-04-12 16:42:58'),
(1322, 1202, 'multiple_choice', 22, 'Which paragraph introduces community-specific design?', 'D', '[\"A\",\"B\",\"C\",\"D\"]', 'Paragraph D mentions local-needs design.', 6.0, 'D', '2026-04-12 16:42:58'),
(1323, 1202, 'sentence_completion', 23, 'Newly arrived residents may need practical online ____.', 'access', NULL, 'Paragraph B reference.', 6.0, 'B', '2026-04-12 16:42:58'),
(1324, 1202, 'matching_heading', 24, 'Best heading for Paragraph C.', 'ii', '[\"i. Reader preference studies\",\"ii. Resource inequality impacts\",\"iii. Innovation awards\",\"iv. Historical archives\"]', 'Paragraph C discusses unequal resources.', 6.5, 'C', '2026-04-12 16:42:58'),
(1325, 1202, 'summary_completion', 25, 'Hybrid service can preserve ____ offerings.', 'traditional', NULL, 'Paragraph D balances old and new service forms.', 6.0, 'D', '2026-04-12 16:42:58'),
(1326, 1202, 'sentence_completion', 26, 'Inclusion outcomes improve when design matches local ____.', 'needs', NULL, 'Paragraph D key phrase.', 6.0, 'D', '2026-04-12 16:42:58'),
(1327, 1202, 'multiple_choice', 27, 'Overall message of the passage is:', 'C', '[\"A. Libraries are obsolete\",\"B. Print-only systems are superior\",\"C. Hybrid adaptation supports communities\",\"D. Funding is no longer important\"]', 'Central argument supports adaptive hybrid model.', 6.5, 'D', '2026-04-12 16:42:58'),
(1328, 1203, 'true_false_not_given', 28, 'Data-driven planning removes all governance concerns.', 'False', '[\"True\",\"False\",\"Not Given\"]', 'Paragraph B says governance concerns remain.', 6.5, 'B', '2026-04-12 16:42:58'),
(1329, 1203, 'true_false_not_given', 29, 'Pilot projects always guarantee successful scaling.', 'False', '[\"True\",\"False\",\"Not Given\"]', 'Paragraph C states scaling needs additional conditions.', 6.5, 'C', '2026-04-12 16:42:58'),
(1330, 1203, 'multiple_choice', 30, 'What is required for scaling pilot schemes?', 'D', '[\"A. More publicity\",\"B. Fewer stakeholders\",\"C. Shorter pilot duration\",\"D. Stable financing and alignment\"]', 'Paragraph C gives both requirements.', 6.5, 'C', '2026-04-12 16:42:58'),
(1331, 1203, 'matching_heading', 31, 'Best heading for Paragraph D.', 'iv', '[\"i. Public opinion conflicts\",\"ii. Outdated systems\",\"iii. Technology procurement\",\"iv. Policy cycle and iterative review\"]', 'Paragraph D details policy cycle stages.', 6.5, 'D', '2026-04-12 16:42:58'),
(1332, 1203, 'sentence_completion', 32, 'Urban planning balances short-term demand and long-term ____.', 'resilience', NULL, 'Paragraph A contrast.', 6.5, 'A', '2026-04-12 16:42:58'),
(1333, 1203, 'summary_completion', 33, 'Data systems can improve intervention ____.', 'precision', NULL, 'Paragraph B explicit term.', 6.5, 'B', '2026-04-12 16:42:58'),
(1334, 1203, 'summary_completion', 34, 'Scaling requires inter-agency ____.', 'alignment', NULL, 'Paragraph C requirement.', 6.5, 'C', '2026-04-12 16:42:58'),
(1335, 1203, 'sentence_completion', 35, 'Policy cycles begin with baseline ____.', 'measurement', NULL, 'Paragraph D first stage.', 6.5, 'D', '2026-04-12 16:42:58'),
(1336, 1203, 'multiple_choice', 36, 'Which issue is paired with trust in Paragraph B?', 'A', '[\"A. Governance concerns\",\"B. Parking limitations\",\"C. Skill shortages\",\"D. Population decline\"]', 'Governance and trust are linked.', 6.5, 'B', '2026-04-12 16:42:58'),
(1337, 1203, 'sentence_completion', 37, 'After baseline, planners use staged ____.', 'rollouts', NULL, 'Paragraph D sequence.', 6.5, 'D', '2026-04-12 16:42:58'),
(1338, 1203, 'summary_completion', 38, 'Periodic ____ supports adaptive policy refinement.', 'review', NULL, 'Paragraph D closing idea.', 6.5, 'D', '2026-04-12 16:42:58'),
(1339, 1203, 'multiple_choice', 39, 'Main theme of Passage 3 is:', 'B', '[\"A. Tourism revenue optimization\",\"B. Adaptive planning under pressure\",\"C. Library modernization\",\"D. Personal transport habits\"]', 'Title and content align with adaptive planning.', 6.5, 'ALL', '2026-04-12 16:42:58'),
(1340, 1203, 'sentence_completion', 40, 'Early pilot success does not eliminate financing ____.', 'requirements', NULL, 'Paragraph C notes financing requirement for scale.', 6.5, 'C', '2026-04-12 16:42:58'),
(2001, 2001, 'true_false_not_given', 1, 'People living near parks have lower rates of heart disease.', 'True', '[\"True\",\"False\",\"Not Given\"]', 'Paragraph A states lower cardiovascular disease.', 6.0, 'A', '2026-05-16 22:45:08'),
(2002, 2001, 'true_false_not_given', 2, 'Cortisol reduction requires at least one hour in nature.', 'False', '[\"True\",\"False\",\"Not Given\"]', 'Paragraph B says within twenty minutes.', 6.0, 'B', '2026-05-16 22:45:08'),
(2003, 2001, 'sentence_completion', 3, 'Some cities have converted old railway lines into ____ parks.', 'linear', NULL, 'Paragraph C mentions linear parks.', 6.0, 'C', '2026-05-16 22:45:08'),
(2004, 2001, 'multiple_choice', 4, 'According to Paragraph D, green space distribution is:', 'B', '[\"A. improving steadily\",\"B. frequently unequal\",\"C. centrally managed\",\"D. no longer a concern\"]', 'Critics note unequal distribution.', 6.5, 'D', '2026-05-16 22:45:08'),
(2005, 2001, 'sentence_completion', 5, 'Investment policies should prioritise ____ communities.', 'underserved', NULL, 'Paragraph D states underserved communities.', 6.0, 'D', '2026-05-16 22:45:08'),
(2006, 2002, 'true_false_not_given', 6, 'During deep sleep the hippocampus replays daytime neural activity.', 'True', '[\"True\",\"False\",\"Not Given\"]', 'Stated directly in Paragraph A.', 6.5, 'A', '2026-05-16 22:45:08'),
(2007, 2002, 'sentence_completion', 7, 'The sleeping group recalled ____% of word pairs correctly.', '92', NULL, 'Paragraph B gives the figure.', 6.5, 'B', '2026-05-16 22:45:08'),
(2008, 2002, 'multiple_choice', 8, 'University students average ____ hours of sleep per night.', 'C', '[\"A. 5.8\",\"B. 7.0\",\"C. 6.2\",\"D. 8.0\"]', 'Paragraph C states 6.2 hours.', 6.5, 'C', '2026-05-16 22:45:08'),
(2009, 2002, 'sentence_completion', 9, 'Each lost hour of sleep was linked to a ____ GPA drop.', '0.15', NULL, 'Paragraph C gives 0.15 drop.', 6.5, 'C', '2026-05-16 22:45:08'),
(2010, 2002, 'true_false_not_given', 10, 'All universities now ban exams before 10 AM.', 'False', '[\"True\",\"False\",\"Not Given\"]', 'Only several institutions, not all.', 6.5, 'D', '2026-05-16 22:45:08'),
(2011, 2003, 'multiple_choice', 11, 'AI diagnostic accuracy compared to specialists is:', 'B', '[\"A. always higher\",\"B. comparable overall\",\"C. much lower\",\"D. untested\"]', 'Meta-analysis shows comparable pooled sensitivity.', 7.0, 'B', '2026-05-16 22:45:08'),
(2012, 2003, 'sentence_completion', 12, 'AI systems produced more false ____ than clinicians.', 'positives', NULL, 'Lower specificity means more false positives.', 7.0, 'B', '2026-05-16 22:45:08'),
(2013, 2003, 'true_false_not_given', 13, 'The FDA now allows continuous AI model updates under a change-control plan.', 'True', '[\"True\",\"False\",\"Not Given\"]', 'Paragraph C describes the FDA pilot programme.', 7.0, 'C', '2026-05-16 22:45:08'),
(2014, 2003, 'sentence_completion', 14, 'Training bias may reduce accuracy for certain ____ groups.', 'ethnic', NULL, 'Paragraph D discusses ethnic group bias.', 7.0, 'D', '2026-05-16 22:45:08'),
(2015, 2003, 'multiple_choice', 15, 'Legal responsibility for AI misdiagnosis is:', 'D', '[\"A. always the developer\",\"B. the hospital\",\"C. the clinician\",\"D. unresolved in most systems\"]', 'Paragraph D says unresolved.', 7.0, 'D', '2026-05-16 22:45:08');

-- --------------------------------------------------------

--
-- Table structure for table `tb_reports`
--

CREATE TABLE `tb_reports` (
  `report_id` int(11) NOT NULL,
  `user_id` int(11) DEFAULT NULL,
  `report_type` enum('bug','feature_request','content_issue','user_report','other') DEFAULT 'other',
  `title` varchar(255) NOT NULL,
  `description` text NOT NULL,
  `priority` enum('low','medium','high','critical') DEFAULT 'medium',
  `status` enum('open','in_progress','resolved','closed') DEFAULT 'open',
  `attachments_json` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`attachments_json`)),
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_reports`
--

INSERT INTO `tb_reports` (`report_id`, `user_id`, `report_type`, `title`, `description`, `priority`, `status`, `attachments_json`, `created_at`, `updated_at`) VALUES
(1, 1, 'bug', 'Cannot load a lesson', 'Lesson page showed empty content in one module.', 'medium', 'closed', '[]', '2026-04-12 04:33:21', '2026-05-18 06:53:56');

-- --------------------------------------------------------

--
-- Table structure for table `tb_roadmap_suggestions`
--

CREATE TABLE `tb_roadmap_suggestions` (
  `suggestion_id` int(11) NOT NULL,
  `teacher_id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `roadmap_id` int(11) NOT NULL,
  `suggestion_title` varchar(255) NOT NULL,
  `message` text NOT NULL,
  `suggested_changes` text NOT NULL,
  `status` enum('pending','accepted','rejected') NOT NULL DEFAULT 'pending',
  `created_at` datetime DEFAULT current_timestamp(),
  `resolved_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_roadmap_suggestions`
--

INSERT INTO `tb_roadmap_suggestions` (`suggestion_id`, `teacher_id`, `student_id`, `roadmap_id`, `suggestion_title`, `message`, `suggested_changes`, `status`, `created_at`, `resolved_at`) VALUES
(2, 8, 3, 3, 'Add more writing example', 'aa', '[{\"action\":\"add\",\"week_number\":2,\"description\":\"a\"}]', '', '2026-05-16 09:51:05', '2026-05-16 16:51:47');

-- --------------------------------------------------------

--
-- Table structure for table `tb_speaking_sessions`
--

CREATE TABLE `tb_speaking_sessions` (
  `session_id` int(11) NOT NULL,
  `user_id` int(11) DEFAULT NULL,
  `topic` text DEFAULT NULL,
  `fluency_score` float DEFAULT NULL,
  `pronunciation_score` float DEFAULT NULL,
  `grammar_score` float DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `audio_url` varchar(500) DEFAULT NULL,
  `transcript` longtext DEFAULT NULL,
  `ai_feedback` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_speaking_sessions`
--

INSERT INTO `tb_speaking_sessions` (`session_id`, `user_id`, `topic`, `fluency_score`, `pronunciation_score`, `grammar_score`, `created_at`, `audio_url`, `transcript`, `ai_feedback`) VALUES
(1, 1, 'Describe your hometown', 6, 6.5, 6, '2026-04-12 04:33:21', 'https://example.com/audio/session1.webm', 'My hometown is in central Vietnam...', 'Good coherence, improve linking words.'),
(2, 3, 'Leisure and media preferences', 6, 5.5, 6, '2026-04-12 10:17:53', 'audio/2.webm', 'hi  are you listening  listening  do you have question mark  Or Not  do you have any girlfriend', 'Good fluency with some hesitation. Work on pronunciation of complex words and reduce filler words like \'um\' and \'uh\'.\n\nStrengths: Good topic development, Natural speech rhythm, Appropriate use of linking words\n\nAreas for Improvement: Reduce filler words, Practice stress patterns, Use more idiomatic expressions'),
(3, 3, 'Daily routines and study/work habits', 6.1, 6.4, 6.6, '2026-04-13 10:22:46', 'audio/3.webm', 'Speaking timer: 00:00', 'Good effort! You communicated your ideas clearly. Areas for improvement: Work on pronunciation of longer words and use more linking words.\n\nStrengths: Clear effort to communicate, Topic relevance maintained\n\nAreas for Improvement: Increase response depth, Improve pronunciation clarity'),
(4, 3, 'Daily routines and study/work habits', 5.6, 5.3, 5.3, '2026-04-14 08:35:18', 'audio/4.webm', 'no', 'Keep practicing! Focus on speaking more fluently without long pauses. Try to expand your vocabulary and practice common IELTS topics regularly.\n\nStrengths: Clear effort to communicate, Topic relevance maintained\n\nAreas for Improvement: Increase response depth, Improve pronunciation clarity'),
(5, 3, 'Public transport in your area', 6, 6, 6.2, '2026-04-14 08:36:31', 'audio/5.webm', 'over rolling nowhere  disconnected', 'Good effort! You communicated your ideas clearly. Areas for improvement: Work on pronunciation of longer words and use more linking words.\n\nStrengths: Clear effort to communicate, Topic relevance maintained\n\nAreas for Improvement: Increase response depth, Improve pronunciation clarity'),
(6, 3, 'Leisure and media preferences', 6, 5.5, 0, '2026-04-19 04:37:54', 'audio/6.webm', 'good morning this is the yield speaking test could you tell me your full name please my name is fine', 'Good effort. Speak in longer turns, reduce hesitation, and improve pronunciation of multisyllabic words.\n\nStrengths: Ideas are understandable, Some natural expressions used\n\nAreas for Improvement: Reduce filler words, Develop answers with reasons and examples'),
(7, 3, 'A useful skill you learned recently', 6, 5.5, 0, '2026-05-15 04:30:22', 'audio/7.webm', 'could you tell me your full name please my name is fine you can start speaking now okay I was born in 5 years ago so I have memorable about my memory my grandma she is very she was a very kind of person okay I\'m done why or why not okay I think technology has been a useful tools to have people travel around the world there\'s somewhere else but we have a radiation call me about the reminder big picture', 'Good effort. Speak in longer turns, reduce hesitation, and improve pronunciation of multisyllabic words.\n\nStrengths: Ideas are understandable, Some natural expressions used\n\nAreas for Improvement: Reduce filler words, Develop answers with reasons and examples'),
(8, 3, 'Food and cooking habits', NULL, NULL, NULL, '2026-05-20 08:32:32', NULL, NULL, NULL),
(9, 3, 'Leisure and media preferences', NULL, NULL, NULL, '2026-05-20 08:34:17', NULL, NULL, NULL),
(10, 3, 'Leisure and media preferences', NULL, NULL, NULL, '2026-05-20 08:36:08', NULL, NULL, NULL),
(11, 3, 'Food and cooking habits', 6, 5.5, 0, '2026-05-20 15:50:20', 'audio/11.webm', 'could you tell me your full name please what do you usually do in your free time play basketball you can start speaking now I would like to talk about my memorable Journey and I had it last year when I travel it to Dallas with three of my close friend doing all summer vacation  we decide to go there because we', 'Good effort. Speak in longer turns, reduce hesitation, and improve pronunciation of multisyllabic words.\n\nStrengths: Ideas are understandable, Some natural expressions used\n\nAreas for Improvement: Reduce filler words, Develop answers with reasons and examples'),
(12, 3, 'Public transport in your area', NULL, NULL, NULL, '2026-05-20 09:17:15', NULL, NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `tb_speaking_topics`
--

CREATE TABLE `tb_speaking_topics` (
  `topic_id` int(11) NOT NULL,
  `part` enum('1','2','3') NOT NULL,
  `topic_title` varchar(255) NOT NULL,
  `description` text DEFAULT NULL,
  `difficulty_level` int(11) DEFAULT 1,
  `band_target` int(11) DEFAULT 6,
  `is_active` tinyint(1) DEFAULT 1,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `status` enum('draft','pending_review','published','rejected') NOT NULL DEFAULT 'draft',
  `created_by` int(11) DEFAULT NULL,
  `reviewed_by` int(11) DEFAULT NULL,
  `reviewed_at` datetime DEFAULT NULL,
  `reviewer_note` text DEFAULT NULL,
  `is_deleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_speaking_topics`
--

INSERT INTO `tb_speaking_topics` (`topic_id`, `part`, `topic_title`, `description`, `difficulty_level`, `band_target`, `is_active`, `created_at`, `updated_at`, `status`, `created_by`, `reviewed_by`, `reviewed_at`, `reviewer_note`, `is_deleted`) VALUES
(1, '2', 'A memorable trip', 'Describe a memorable trip you had in your life.', 2, 6, 1, '2026-04-12 04:33:21', '2026-05-16 08:06:51', 'published', NULL, 8, '2026-05-16 08:06:51', NULL, 0),
(1501, '1', 'Hometown and local facilities', 'Talk about where you live and what local services people use most often.', 2, 7, 1, '2026-04-12 09:42:58', '2026-05-16 02:16:50', 'draft', NULL, NULL, NULL, NULL, 0),
(1502, '1', 'Daily routines and study/work habits', 'Describe your regular weekday routine and how you manage your schedule.', 2, 6, 1, '2026-04-12 09:42:58', '2026-05-16 15:52:30', 'published', NULL, 8, '2026-05-16 15:52:30', NULL, 0),
(1503, '1', 'Leisure and media preferences', 'Discuss your free-time activities and what media you consume.', 2, 6, 1, '2026-04-12 09:42:58', '2026-05-20 16:30:41', 'published', NULL, 8, '2026-05-20 16:30:41', NULL, 0),
(1504, '1', 'Public transport in your area', 'Explain how people usually travel in your city and why.', 2, 6, 1, '2026-04-12 09:42:58', '2026-04-12 09:42:58', 'draft', NULL, NULL, NULL, NULL, 0),
(1505, '2', 'A useful skill you learned recently', 'Describe a useful skill you learned recently. You should say what it is, how you learned it, and why it is useful.', 3, 6, 1, '2026-04-12 09:42:58', '2026-04-12 09:42:58', 'draft', NULL, NULL, NULL, NULL, 0),
(1506, '2', 'A place that became popular in your city', 'Describe a place that has become popular recently in your city and explain why people like it.', 3, 6, 1, '2026-04-12 09:42:58', '2026-05-16 15:52:47', 'published', NULL, 8, '2026-05-16 15:52:47', NULL, 0),
(1507, '2', 'A memorable journey', 'Describe a memorable journey you had. You should say where you went, who you were with, and why it was memorable.', 3, 6, 1, '2026-04-12 09:42:58', '2026-04-12 09:42:58', 'draft', NULL, NULL, NULL, NULL, 0),
(1508, '2', 'A person who influenced your thinking', 'Describe someone who influenced the way you think about life or study.', 3, 7, 1, '2026-04-12 09:42:58', '2026-04-12 09:42:58', 'draft', NULL, NULL, NULL, NULL, 0),
(1509, '3', 'Technology and social interaction', 'Discuss whether digital communication strengthens or weakens real human relationships.', 3, 7, 1, '2026-04-12 09:42:58', '2026-04-12 09:42:58', 'draft', NULL, NULL, NULL, NULL, 0),
(1510, '3', 'Urban transport and quality of life', 'Discuss how transport planning affects productivity, health, and equality.', 3, 7, 1, '2026-04-12 09:42:58', '2026-05-16 15:53:05', 'rejected', NULL, 8, '2026-05-16 15:53:05', NULL, 0),
(1511, '3', 'Education and social mobility', 'Discuss whether education systems can reduce long-term inequality.', 4, 7, 1, '2026-04-12 09:42:58', '2026-04-12 09:42:58', 'draft', NULL, NULL, NULL, NULL, 0),
(1512, '3', 'Balancing economic growth and sustainability', 'Discuss how governments should balance growth targets with environmental concerns.', 4, 7, 1, '2026-04-12 09:42:58', '2026-04-12 09:42:58', 'draft', NULL, NULL, NULL, NULL, 0),
(2001, '1', 'Food and cooking habits', 'Questions about what you eat, whether you cook, and food preferences in your country.', 2, 6, 1, '2026-05-16 15:45:17', '2026-05-16 15:45:17', 'published', NULL, NULL, NULL, NULL, 0),
(2002, '2', 'A time you helped someone', 'Describe a time when you helped someone who really needed it. You should say who the person was, what the situation was, how you helped, and explain how you felt afterwards.', 3, 7, 1, '2026-05-16 15:45:17', '2026-05-16 15:52:13', 'published', NULL, 8, '2026-05-16 15:52:13', NULL, 0),
(2003, '3', 'Volunteering and community service', 'Discussion questions about the value of volunteering, whether it should be compulsory, and how communities benefit from unpaid work.', 4, 7, 1, '2026-05-16 15:45:17', '2026-05-16 15:45:17', 'published', NULL, NULL, NULL, NULL, 0),
(2004, '2', 'Describe a best journey you&amp;#39;ve had', NULL, 1, 5, 1, '2026-05-20 09:45:58', '2026-05-20 16:46:04', 'pending_review', 8, NULL, NULL, NULL, 0);

-- --------------------------------------------------------

--
-- Table structure for table `tb_speaking_topic_parts`
--

CREATE TABLE `tb_speaking_topic_parts` (
  `part_id` int(11) NOT NULL,
  `topic_id` int(11) NOT NULL,
  `part_number` int(11) DEFAULT NULL,
  `content_text` text DEFAULT NULL,
  `time_limit_seconds` int(11) DEFAULT NULL,
  `sequence_order` int(11) DEFAULT NULL,
  `is_follow_up` tinyint(1) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_speaking_topic_parts`
--

INSERT INTO `tb_speaking_topic_parts` (`part_id`, `topic_id`, `part_number`, `content_text`, `time_limit_seconds`, `sequence_order`, `is_follow_up`) VALUES
(1, 1, 2, 'You should say: where you went, who you went with, and why it was memorable.', 120, 1, 0),
(1601, 1501, 1, 'What do you like most about your hometown?', 35, 1, 0),
(1602, 1501, 1, 'Has your hometown changed over the past few years?', 35, 2, 1),
(1603, 1502, 1, 'Do you prefer studying/working in the morning or at night?', 35, 1, 0),
(1604, 1502, 1, 'How do you organize your time on busy days?', 35, 2, 1),
(1605, 1503, 1, 'What kind of entertainment do you prefer after work or study?', 35, 1, 0),
(1606, 1504, 1, 'Is public transport convenient in your area?', 35, 1, 0),
(1607, 1505, 2, 'You should say: what the skill is, how you learned it, when you use it, and explain why it is useful.', 120, 1, 0),
(1608, 1505, 2, 'Would you recommend this skill to other people? Why?', 35, 2, 1),
(1609, 1506, 2, 'You should say: where the place is, what people do there, and why it became popular.', 120, 1, 0),
(1610, 1506, 2, 'Do you think popularity changes a place positively or negatively?', 35, 2, 1),
(1611, 1507, 2, 'You should say: where you went, what happened, and why you still remember this journey.', 120, 1, 0),
(1612, 1508, 2, 'You should say: who this person is, what ideas they influenced, and how this affected your decisions.', 120, 1, 0),
(1613, 1509, 3, 'Can online interaction replace face-to-face communication in the future?', 70, 1, 0),
(1614, 1509, 3, 'How should schools teach responsible digital communication?', 70, 2, 1),
(1615, 1510, 3, 'Which transport policies should cities prioritize to reduce congestion?', 70, 1, 0),
(1616, 1510, 3, 'Do you think private cars will become less common in big cities?', 70, 2, 1),
(1617, 1511, 3, 'How can education reduce inequality between social groups?', 70, 1, 0),
(1618, 1512, 3, 'Should governments accept slower growth for environmental protection?', 70, 1, 0),
(1619, 1512, 3, 'What role should individuals play in sustainability transitions?', 70, 2, 1),
(2001, 2001, 1, 'Do you enjoy cooking? Why or why not?', 35, 1, 0),
(2002, 2001, 1, 'What is a typical meal in your country?', 35, 2, 1),
(2003, 2001, 1, 'Do you think people eat more healthily now than in the past?', 35, 3, 1),
(2004, 2002, 2, 'You should say: who you helped, what the problem was, what you did to help, and explain how you felt about it afterwards.', 120, 1, 0),
(2005, 2002, 2, 'Would you help that person again in the same way?', 35, 2, 1),
(2006, 2003, 3, 'Why do some people volunteer while others do not?', 70, 1, 0),
(2007, 2003, 3, 'Should schools require students to do community service?', 70, 2, 1),
(2008, 2003, 3, 'How can governments encourage more people to volunteer?', 70, 3, 1);

-- --------------------------------------------------------

--
-- Table structure for table `tb_system_settings`
--

CREATE TABLE `tb_system_settings` (
  `setting_key` varchar(255) NOT NULL,
  `setting_value` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_system_settings`
--

INSERT INTO `tb_system_settings` (`setting_key`, `setting_value`) VALUES
('site_name', 'WebIeltsFree'),
('site_description', 'Free online IELTS preparation with advanced AI evaluations and professional tutoring'),
('allow_registration', 'true'),
('minimum_band_target', '4.0');

-- --------------------------------------------------------

--
-- Table structure for table `tb_teacher_assignments`
--

CREATE TABLE `tb_teacher_assignments` (
  `assignment_id` int(11) NOT NULL,
  `test_id` int(11) NOT NULL,
  `teacher_id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `title` varchar(255) NOT NULL,
  `instructions` text DEFAULT NULL,
  `deadline` datetime DEFAULT NULL,
  `status` enum('pending','completed','overdue') NOT NULL DEFAULT 'pending',
  `assigned_at` datetime DEFAULT current_timestamp(),
  `completed_at` datetime DEFAULT NULL,
  `attempt_id` int(11) DEFAULT NULL,
  `is_deleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_teacher_assignments`
--

INSERT INTO `tb_teacher_assignments` (`assignment_id`, `test_id`, `teacher_id`, `student_id`, `title`, `instructions`, `deadline`, `status`, `assigned_at`, `completed_at`, `attempt_id`, `is_deleted`) VALUES
(2, 601, 6, 3, 'Weekly Academic Test', NULL, NULL, 'completed', '2026-05-16 05:49:43', '2026-05-21 02:30:58', 4, 0),
(3, 2004, 8, 9, 'a', 'a', NULL, 'pending', '2026-05-16 15:49:14', NULL, NULL, 0),
(4, 2001, 8, 9, 'Weekly', 'aaa', '2026-05-21 01:00:00', 'overdue', '2026-05-21 04:13:47', NULL, NULL, 0);

-- --------------------------------------------------------

--
-- Table structure for table `tb_teacher_certificates`
--

CREATE TABLE `tb_teacher_certificates` (
  `certificate_id` int(11) NOT NULL,
  `teacher_id` int(11) NOT NULL,
  `title` varchar(255) NOT NULL DEFAULT '',
  `image_url` varchar(255) NOT NULL DEFAULT '',
  `issue_date` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tb_teacher_profiles`
--

CREATE TABLE `tb_teacher_profiles` (
  `teacher_id` int(11) NOT NULL,
  `bio` text DEFAULT NULL,
  `specialties` varchar(500) DEFAULT NULL,
  `years_experience` int(11) DEFAULT NULL,
  `is_public` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` datetime DEFAULT current_timestamp(),
  `updated_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_teacher_profiles`
--

INSERT INTO `tb_teacher_profiles` (`teacher_id`, `bio`, `specialties`, `years_experience`, `is_public`, `created_at`, `updated_at`) VALUES
(6, 'Full Name: Emma L. Clark\r\nSpecialties: Writing Task 2, Speaking Strategy, Academic Vocabulary\r\nYears of Experience: 6 years\r\n&quot;Hello everyone, I am Emma L. Clark. With over 6 years of specialized experience in teaching and preparing students for the IELTS Academic exam, I have helped more than 1,000 students successfully jump from a 5.0 to a 7.5+ band score.\r\n\r\nMy teaching philosophy focuses on optimizing linguistic thinking rather than rote memorization of sample essays. At WebIeltsFree, my primary responsibilities include:\r\n\r\nAuditing and approving new Writing and Speaking practice materials.\r\n\r\nDirectly grading and resolving disputes regarding AI-generated test results to ensure absolute accuracy.\r\n\r\nProviding personalized learning roadmaps tailored to each student&#39;s actual proficiency level.\r\n\r\nCredentials &amp; Certifications:\r\n\r\nIELTS Academic: 8.0 Overall (Listening 8.5, Reading 8.0, Writing 7.5, Speaking 8.0).\r\n\r\nTESOL International Teaching Certification.\r\n\r\nBachelor’s Degree in English Education.\r\n\r\nI believe that by combining modern AI technology with the practical expertise of our teaching staff, you can conquer your IELTS goals faster and more accurately than ever before.&quot;', 'I have a 8.0 ielts certificate', 6, 1, '2026-05-16 05:57:58', '2026-05-16 06:03:10'),
(8, '&amp;quot;Hello everyone, I am Vũ Th&amp;#249;y. With over 6 years of specialized experience in teaching and preparing students for the IELTS Academic exam, I have helped more than 1,000 students successfully jump from a 5.0 to a 7.5+ band score.\r\n\r\nMy teaching philosophy focuses on optimizing linguistic thinking rather than rote memorization of sample essays. At WebIeltsFree, my primary responsibilities include:\r\n\r\nAuditing and approving new Writing and Speaking practice materials.\r\n\r\nDirectly grading and resolving disputes regarding AI-generated test results to ensure absolute accuracy.\r\n\r\nProviding personalized learning roadmaps tailored to each student&amp;#39;s actual proficiency level.\r\n\r\nCredentials &amp;amp; Certifications:\r\n\r\nIELTS Academic: 8.0 Overall (Listening 8.5, Reading 8.0, Writing 7.5, Speaking 8.0).\r\n\r\nTESOL International Teaching Certification.\r\n\r\nBachelor’s Degree in English Education.\r\n\r\nI believe that by combining modern AI technology with the practical expertise of our teaching staff, you can conquer your IELTS goals faster and more accurately than ever before.&amp;quot;', '8.0 IELTS Certificate', 6, 1, '2026-05-16 08:03:39', '2026-05-20 16:30:58');

-- --------------------------------------------------------

--
-- Table structure for table `tb_tests`
--

CREATE TABLE `tb_tests` (
  `test_id` int(11) NOT NULL,
  `title` varchar(255) DEFAULT NULL,
  `difficulty` int(11) DEFAULT NULL,
  `duration_minutes` int(11) DEFAULT NULL,
  `is_deleted` tinyint(1) DEFAULT 0,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `created_by` int(11) DEFAULT NULL COMMENT 'FK to tb_users (teacher)',
  `is_teacher_created` tinyint(1) NOT NULL DEFAULT 0,
  `is_public` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_tests`
--

INSERT INTO `tb_tests` (`test_id`, `title`, `difficulty`, `duration_minutes`, `is_deleted`, `deleted_at`, `updated_at`, `created_by`, `is_teacher_created`, `is_public`) VALUES
(1, 'IELTS Reading Test 1', 2, 60, 0, NULL, '2026-04-12 04:33:20', NULL, 0, 1),
(2, 'IELTS Listening Test 1', 2, 40, 0, NULL, '2026-04-12 04:33:20', NULL, 0, 1),
(3, 'IELTS Writing Test 1', 3, 60, 0, NULL, '2026-04-12 04:33:20', NULL, 0, 1),
(4, 'IELTS Speaking Test 1', 2, 14, 0, NULL, '2026-04-12 04:33:20', NULL, 0, 1),
(601, 'IELTS Academic Listening Mock A (40Q)', 3, 40, 0, NULL, '2026-04-12 09:42:57', NULL, 0, 1),
(602, 'IELTS Academic Reading Mock A (40Q)', 3, 60, 0, NULL, '2026-04-12 09:42:57', NULL, 0, 1),
(603, 'IELTS Academic Writing Mock A (Task 1+2)', 3, 60, 0, NULL, '2026-04-12 09:42:57', NULL, 0, 1),
(604, 'IELTS Academic Speaking Mock A (Part 1-3)', 3, 14, 0, NULL, '2026-04-12 09:42:57', NULL, 0, 1),
(605, 'IELTS Academic Full Mock A (Integrated)', 4, 174, 0, NULL, '2026-04-12 09:42:57', NULL, 0, 1),
(2001, 'Cambridge 20 - Listening Test 1', 3, 30, 0, NULL, '2026-05-16 15:44:50', NULL, 0, 1),
(2002, 'Cambridge 20 - Reading Test 1', 3, 60, 0, NULL, '2026-05-16 15:44:50', NULL, 0, 1),
(2003, 'Cambridge 20 - Writing Test 1', 3, 60, 0, NULL, '2026-05-16 15:44:50', NULL, 0, 1),
(2004, 'Cambridge 20 - Speaking Test 1', 3, 14, 0, NULL, '2026-05-16 15:44:50', NULL, 0, 1),
(2005, 'Cambridge 20 - Full Academic Test 1', 4, 174, 0, NULL, '2026-05-16 15:44:50', NULL, 0, 1),
(2008, 'aa', 6, 60, 0, NULL, NULL, 8, 1, 1);

-- --------------------------------------------------------

--
-- Table structure for table `tb_test_sections`
--

CREATE TABLE `tb_test_sections` (
  `section_id` int(11) NOT NULL,
  `test_id` int(11) DEFAULT NULL,
  `skill_type` enum('reading','listening','writing','speaking') DEFAULT NULL,
  `audio_url` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_test_sections`
--

INSERT INTO `tb_test_sections` (`section_id`, `test_id`, `skill_type`, `audio_url`) VALUES
(1, 1, 'reading', NULL),
(2, 2, 'listening', '/audio/cam20/T1S1.m4a'),
(3, 3, 'writing', NULL),
(4, 4, 'speaking', NULL),
(701, 601, 'listening', '/audio/cam20/T1S1.m4a'),
(702, 602, 'reading', NULL),
(703, 603, 'writing', NULL),
(704, 604, 'speaking', NULL),
(705, 605, 'listening', '/audio/cam20/T1S1.m4a'),
(706, 605, 'reading', NULL),
(707, 605, 'writing', NULL),
(708, 605, 'speaking', NULL),
(2001, 2001, 'listening', '/audio/cam20/T1S1.m4a'),
(2002, 2002, 'reading', NULL),
(2003, 2003, 'writing', NULL),
(2004, 2004, 'speaking', NULL),
(2005, 2005, 'listening', '/audio/cam20/T2S1.m4a'),
(2006, 2005, 'reading', NULL),
(2007, 2005, 'writing', NULL),
(2008, 2005, 'speaking', NULL),
(2011, 2008, 'reading', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `tb_users`
--

CREATE TABLE `tb_users` (
  `user_id` int(11) NOT NULL,
  `email` varchar(255) DEFAULT NULL,
  `username` varchar(50) DEFAULT NULL,
  `password_hash` varchar(255) DEFAULT NULL,
  `role` enum('student','teacher','admin','moderator') NOT NULL DEFAULT 'student',
  `status` enum('active','suspended') DEFAULT NULL,
  `has_completed_onboarding` tinyint(1) DEFAULT 0,
  `email_verified` tinyint(1) DEFAULT 0,
  `email_verification_token` varchar(255) DEFAULT NULL,
  `last_login_at` timestamp NULL DEFAULT NULL,
  `login_count` int(11) DEFAULT 0,
  `is_deleted` tinyint(1) DEFAULT 0,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `reset_password_token` varchar(255) DEFAULT NULL,
  `reset_password_expires` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_users`
--

INSERT INTO `tb_users` (`user_id`, `email`, `username`, `password_hash`, `role`, `status`, `has_completed_onboarding`, `email_verified`, `email_verification_token`, `last_login_at`, `login_count`, `is_deleted`, `deleted_at`, `created_at`, `updated_at`, `reset_password_token`, `reset_password_expires`) VALUES
(1, 'student1@example.com', 'student1', '$2a$11$demo_hash_value', 'student', '', 1, 1, NULL, NULL, 5, 0, NULL, '2026-04-12 04:33:20', '2026-05-16 15:47:39', NULL, NULL),
(2, 'admin1@example.com', 'admin1', '$2a$11$demo_hash_value', 'admin', 'active', 1, 1, NULL, NULL, 12, 0, NULL, '2026-04-12 04:33:20', '2026-04-12 04:33:20', NULL, NULL),
(3, 'dogeinabath2005@gmail.com', NULL, '$2a$11$40y760xIO/WAZdqn9x28PO87Q/s7Rlnyav/2Z5pm4qdH88HtoufeW', 'student', 'active', 1, 0, NULL, NULL, 0, 0, NULL, '2026-04-11 22:18:37', '2026-04-12 05:18:47', NULL, NULL),
(4, 'dogeinabath20055@gmail.com', 'admin', 'admin123', 'admin', 'active', 0, 0, NULL, NULL, 0, 0, NULL, '2026-05-15 16:27:20', '2026-05-15 16:27:20', NULL, NULL),
(5, 'admin@webieltsfree.com', 'Administrator', '$2a$11$R9h/lIPzHZ7pJLsWmtEuce5MTuWq3shCQpL9.I8Fas5.889.B/V6e', 'admin', 'active', 0, 0, NULL, NULL, 0, 0, NULL, '2026-05-15 16:29:39', '2026-05-15 16:29:39', NULL, NULL),
(6, 'superadmin@dev.local', 'superadmin', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'teacher', 'active', 1, 1, NULL, NULL, 0, 0, NULL, '2026-05-15 17:43:26', '2026-05-16 05:50:44', NULL, NULL),
(7, 'namkhanh2795@gmail.com', NULL, '$2a$11$Fecs9u/1j42bbFvs6uisQ.G.eGfyO.Ktglz2rVIC8dCrJEFfv9lmS', 'admin', 'active', 1, 1, NULL, NULL, 0, 0, NULL, '2026-05-16 00:59:09', '2026-05-18 13:54:49', NULL, NULL),
(8, 'vuthuy123@gmail.com', NULL, '$2a$11$1Jkd7vW1oJjQAw0cEK2C7.x4.qtQ.Otfv226Kmez6ZMVyXgto0z8q', 'teacher', 'active', 0, 1, NULL, NULL, 0, 0, NULL, '2026-05-16 01:00:23', NULL, NULL, NULL),
(9, 'syamonguyen2005@gmail.com', NULL, '$2a$11$OvHAWPuidm1MwGF4EymmteMVi.6WfbPuYLr1JIDn7uNIGpYwdsS0.', 'student', 'active', 1, 0, NULL, NULL, 0, 0, NULL, '2026-05-16 02:57:01', '2026-05-16 10:01:24', NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `tb_user_achievements`
--

CREATE TABLE `tb_user_achievements` (
  `user_achievement_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `achievement_id` int(11) NOT NULL,
  `unlocked_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_user_achievements`
--

INSERT INTO `tb_user_achievements` (`user_achievement_id`, `user_id`, `achievement_id`, `unlocked_at`) VALUES
(1, 1, 1, '2026-04-12 04:33:21');

-- --------------------------------------------------------

--
-- Table structure for table `tb_user_answers`
--

CREATE TABLE `tb_user_answers` (
  `id` int(11) NOT NULL,
  `attempt_id` int(11) DEFAULT NULL,
  `question_id` int(11) DEFAULT NULL,
  `user_answer` text DEFAULT NULL,
  `is_correct` tinyint(1) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_user_answers`
--

INSERT INTO `tb_user_answers` (`id`, `attempt_id`, `question_id`, `user_answer`, `is_correct`) VALUES
(1, 1, 1, 'True', 1),
(2, 2, 1, 'aa', 0),
(3, 3, 2, 'aaa', 0),
(4, 4, 8001, '', 0),
(5, 4, 8002, '', 0),
(6, 4, 8003, '', 0),
(7, 4, 8004, '', 0),
(8, 4, 8005, '', 0),
(9, 4, 8006, '', 0),
(10, 4, 8007, '', 0),
(11, 4, 8008, '', 0),
(12, 4, 8009, '', 0),
(13, 4, 8010, '', 0),
(14, 4, 8011, '', 0),
(15, 4, 8012, '', 0),
(16, 4, 8013, '', 0),
(17, 4, 8014, '', 0),
(18, 4, 8015, '', 0),
(19, 4, 8016, '', 0),
(20, 4, 8017, '', 0),
(21, 4, 8018, '', 0),
(22, 4, 8019, '', 0),
(23, 4, 8020, '', 0),
(24, 4, 8021, '', 0),
(25, 4, 8022, '', 0),
(26, 4, 8023, '', 0),
(27, 4, 8024, '', 0),
(28, 4, 8025, '', 0),
(29, 4, 8026, '', 0),
(30, 4, 8027, '', 0),
(31, 4, 8028, '', 0),
(32, 4, 8029, '', 0),
(33, 4, 8030, '', 0),
(34, 4, 8031, '', 0),
(35, 4, 8032, '', 0),
(36, 4, 8033, '', 0),
(37, 4, 8034, '', 0),
(38, 4, 8035, '', 0),
(39, 4, 8036, '', 0),
(40, 4, 8037, '', 0),
(41, 4, 8038, '', 0),
(42, 4, 8039, '', 0),
(43, 4, 8040, '', 0),
(44, 5, 1, 'a', 0),
(45, 6, 2, 'a', 0);

-- --------------------------------------------------------

--
-- Table structure for table `tb_user_daily_activity`
--

CREATE TABLE `tb_user_daily_activity` (
  `activity_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `activity_date` date NOT NULL,
  `lessons_completed` int(11) DEFAULT 0,
  `questions_answered` int(11) DEFAULT 0,
  `writing_submissions` int(11) DEFAULT 0,
  `speaking_sessions` int(11) DEFAULT 0,
  `minutes_spent` int(11) DEFAULT 0,
  `xp_earned` int(11) DEFAULT 0,
  `streak_day` int(11) DEFAULT 0,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_user_daily_activity`
--

INSERT INTO `tb_user_daily_activity` (`activity_id`, `user_id`, `activity_date`, `lessons_completed`, `questions_answered`, `writing_submissions`, `speaking_sessions`, `minutes_spent`, `xp_earned`, `streak_day`, `created_at`, `updated_at`) VALUES
(1, 1, '2026-04-12', 1, 10, 1, 1, 45, 120, 1, '2026-04-12 04:33:21', '2026-04-12 04:33:21');

-- --------------------------------------------------------

--
-- Table structure for table `tb_user_goals`
--

CREATE TABLE `tb_user_goals` (
  `goal_id` int(11) NOT NULL,
  `user_id` int(11) DEFAULT NULL,
  `current_band` float DEFAULT NULL,
  `target_band` float DEFAULT NULL,
  `exam_date` date DEFAULT NULL,
  `study_hours_per_day` int(11) DEFAULT NULL,
  `learning_reason` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_user_goals`
--

INSERT INTO `tb_user_goals` (`goal_id`, `user_id`, `current_band`, `target_band`, `exam_date`, `study_hours_per_day`, `learning_reason`) VALUES
(1, 1, 5.5, 7, '2026-12-15', 2, 'Du hoc'),
(2, 3, 0.5, 7, '2026-08-21', 2, 'academic'),
(3, 6, NULL, 7, '2026-08-16', 2, 'academic'),
(4, 9, 0.5, 8, '2026-08-16', 2, 'academic');

-- --------------------------------------------------------

--
-- Table structure for table `tb_user_learning_progress`
--

CREATE TABLE `tb_user_learning_progress` (
  `id` int(11) NOT NULL,
  `user_id` int(11) DEFAULT NULL,
  `lesson_id` int(11) DEFAULT NULL,
  `completion_percent` float DEFAULT NULL,
  `score` float DEFAULT NULL,
  `last_accessed` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_user_learning_progress`
--

INSERT INTO `tb_user_learning_progress` (`id`, `user_id`, `lesson_id`, `completion_percent`, `score`, `last_accessed`) VALUES
(1, 1, 1, 75, 80, '2026-04-12 04:33:21'),
(2, 3, 4, 100, NULL, '2026-04-11 22:24:07');

-- --------------------------------------------------------

--
-- Table structure for table `tb_user_placement_results`
--

CREATE TABLE `tb_user_placement_results` (
  `result_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `test_id` int(11) DEFAULT NULL,
  `reading_score` int(11) DEFAULT NULL,
  `listening_score` int(11) DEFAULT NULL,
  `writing_score` int(11) DEFAULT NULL,
  `speaking_score` int(11) DEFAULT NULL,
  `overall_band` decimal(2,1) DEFAULT NULL,
  `completed_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `recommendations` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_user_placement_results`
--

INSERT INTO `tb_user_placement_results` (`result_id`, `user_id`, `test_id`, `reading_score`, `listening_score`, `writing_score`, `speaking_score`, `overall_band`, `completed_at`, `recommendations`) VALUES
(1, 1, 1, 28, 30, 22, 23, 6.5, '2026-04-12 04:33:21', 'Focus on writing coherence and speaking fluency.'),
(2, 9, NULL, 0, 0, 0, 0, 0.5, '2026-05-16 07:31:23', 'listening: ưu tiên luyện nền tảng theo dạng câu hỏi sai nhiều. reading: ưu tiên luyện nền tảng theo dạng câu hỏi sai nhiều. writing: ưu tiên luyện nền tảng theo dạng câu hỏi sai nhiều. speaking: ưu tiên luyện nền tảng theo dạng câu hỏi sai nhiều.'),
(3, 9, NULL, 0, 0, 0, 0, 0.5, '2026-05-16 08:09:13', 'listening: ưu tiên luyện nền tảng theo dạng câu hỏi sai nhiều. reading: ưu tiên luyện nền tảng theo dạng câu hỏi sai nhiều. writing: ưu tiên luyện nền tảng theo dạng câu hỏi sai nhiều. speaking: ưu tiên luyện nền tảng theo dạng câu hỏi sai nhiều.'),
(4, 3, NULL, 0, 0, 0, 0, 0.5, '2026-05-20 19:31:52', 'listening: ưu tiên luyện nền tảng theo dạng câu hỏi sai nhiều. reading: ưu tiên luyện nền tảng theo dạng câu hỏi sai nhiều. writing: ưu tiên luyện nền tảng theo dạng câu hỏi sai nhiều. speaking: ưu tiên luyện nền tảng theo dạng câu hỏi sai nhiều.');

-- --------------------------------------------------------

--
-- Table structure for table `tb_user_practice_attempts`
--

CREATE TABLE `tb_user_practice_attempts` (
  `attempt_id` int(11) NOT NULL,
  `user_id` int(11) DEFAULT NULL,
  `skill_type` varchar(20) NOT NULL,
  `listening_question_id` int(11) DEFAULT NULL,
  `reading_question_id` int(11) DEFAULT NULL,
  `user_answer` text DEFAULT NULL,
  `is_correct` tinyint(1) DEFAULT 0,
  `time_spent_seconds` int(11) DEFAULT NULL,
  `attempt_date` datetime DEFAULT current_timestamp(),
  `attempt_number` int(11) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_user_practice_attempts`
--

INSERT INTO `tb_user_practice_attempts` (`attempt_id`, `user_id`, `skill_type`, `listening_question_id`, `reading_question_id`, `user_answer`, `is_correct`, `time_spent_seconds`, `attempt_date`, `attempt_number`) VALUES
(1, 1, 'reading', NULL, 1, 'True', 1, 95, '2026-04-12 11:33:21', 1),
(2, 3, 'listening', 1, NULL, '10 PM', 1, 60, '2026-04-12 15:33:44', 1),
(3, 3, 'reading', NULL, 1301, 'True', 0, 60, '2026-04-13 03:18:19', 1),
(4, 3, 'listening', 1, NULL, '9 PM', 0, 60, '2026-04-13 03:48:41', 1),
(5, 3, 'reading', NULL, 1301, 'True', 0, 60, '2026-04-14 09:41:42', 1),
(6, 3, 'reading', NULL, 1301, 'True', 0, 60, '2026-04-14 09:41:42', 1),
(7, 3, 'reading', NULL, 1301, 'True', 0, 60, '2026-05-12 09:04:49', 1),
(8, 9, 'reading', NULL, 1301, 'True', 0, 60, '2026-05-16 15:46:12', 1),
(9, 9, 'listening', 1, NULL, '9 PM', 0, 60, '2026-05-16 15:47:10', 1),
(10, 3, 'reading', NULL, 1301, 'False', 0, 60, '2026-05-18 15:59:32', 1),
(11, 3, 'listening', 1, NULL, '9 PM', 0, 60, '2026-05-19 13:47:03', 1),
(12, 3, 'reading', NULL, 1301, 'True', 0, 60, '2026-05-19 13:49:26', 1),
(13, 3, 'reading', NULL, 1301, 'False', 0, 60, '2026-05-20 05:51:11', 1),
(14, 3, 'listening', 1, NULL, '9 PM', 0, 60, '2026-05-20 16:17:03', 1),
(15, 3, 'reading', NULL, 1301, 'True', 0, 60, '2026-05-21 00:56:03', 1),
(16, 3, 'reading', NULL, 1301, 'Not Given', 1, 60, '2026-05-21 00:56:12', 1),
(17, 3, 'reading', NULL, 1301, 'True', 0, 60, '2026-05-21 01:05:23', 1),
(18, 3, 'reading', NULL, 1301, 'True', 0, 60, '2026-05-21 01:26:33', 1),
(19, 3, 'reading', NULL, 1301, 'True', 0, 60, '2026-05-21 01:26:51', 1),
(20, 3, 'reading', NULL, 1301, 'True', 0, 60, '2026-05-21 01:27:10', 1),
(21, 3, 'listening', 2003, NULL, 'a', 0, 15, '2026-05-21 01:53:01', 1),
(22, 3, 'listening', 2005, NULL, 'a', 0, 15, '2026-05-21 01:53:01', 1),
(23, 3, 'listening', 2001, NULL, 'a', 0, 15, '2026-05-21 01:53:01', 1),
(24, 3, 'listening', 2004, NULL, 'a', 0, 15, '2026-05-21 01:53:01', 1),
(25, 3, 'listening', 2002, NULL, 'a', 0, 15, '2026-05-21 01:53:01', 1),
(26, 3, 'listening', 2001, NULL, 'a', 0, 15, '2026-05-21 01:53:16', 1),
(27, 3, 'listening', 2003, NULL, 'a', 0, 15, '2026-05-21 01:53:16', 1),
(28, 3, 'listening', 2004, NULL, 'a', 0, 15, '2026-05-21 01:53:16', 1),
(29, 3, 'listening', 2002, NULL, 'a', 0, 15, '2026-05-21 01:53:16', 1),
(30, 3, 'listening', 2005, NULL, 'a', 0, 15, '2026-05-21 01:53:16', 1),
(31, 3, 'listening', 2006, NULL, 'a', 0, 15, '2026-05-21 01:53:16', 1),
(32, 3, 'listening', 2007, NULL, 'a', 0, 15, '2026-05-21 01:53:16', 1),
(33, 3, 'listening', 2008, NULL, 'a', 0, 15, '2026-05-21 01:53:16', 1),
(34, 3, 'listening', 2009, NULL, 'A', 0, 15, '2026-05-21 01:53:16', 1),
(35, 3, 'listening', 2010, NULL, 'a', 0, 15, '2026-05-21 01:53:16', 1),
(36, 3, 'reading', NULL, 1312, 'A', 0, 15, '2026-05-21 01:53:59', 1),
(37, 3, 'reading', NULL, 2002, 'A', 0, 15, '2026-05-21 02:12:15', 1),
(38, 3, 'reading', NULL, 2004, 'C', 0, 15, '2026-05-21 02:12:15', 1);

-- --------------------------------------------------------

--
-- Table structure for table `tb_user_profiles`
--

CREATE TABLE `tb_user_profiles` (
  `user_id` int(11) NOT NULL,
  `full_name` varchar(255) DEFAULT NULL,
  `avatar_url` text DEFAULT NULL,
  `country` varchar(100) DEFAULT NULL,
  `timezone` varchar(100) DEFAULT NULL,
  `preferred_language` varchar(50) DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_user_profiles`
--

INSERT INTO `tb_user_profiles` (`user_id`, `full_name`, `avatar_url`, `country`, `timezone`, `preferred_language`, `created_at`) VALUES
(1, 'Nguyen Van A', 'https://example.com/avatar-student.png', 'VN', 'Asia/Ho_Chi_Minh', 'vi', '2026-04-12 04:33:20'),
(2, 'System Admin', 'https://example.com/avatar-admin.png', 'VN', 'Asia/Ho_Chi_Minh', 'vi', '2026-04-12 04:33:20'),
(3, 'Nguyễn Nam Khánh', NULL, NULL, NULL, NULL, '2026-04-11 22:18:37'),
(6, 'Super Admin (Dev)', NULL, NULL, NULL, NULL, '2026-05-15 17:43:26'),
(7, 'namkhanh2795', NULL, NULL, NULL, NULL, '2026-05-16 00:59:09'),
(8, 'vuthuy123', '/uploads/avatars/avatar_8_639148914586623579.svg', NULL, NULL, NULL, '2026-05-16 01:00:23'),
(9, 'Nguyễn Sỹ Mao', NULL, NULL, NULL, NULL, '2026-05-16 02:57:01');

-- --------------------------------------------------------

--
-- Table structure for table `tb_user_sessions`
--

CREATE TABLE `tb_user_sessions` (
  `session_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `refresh_token` varchar(500) NOT NULL,
  `token_family` varchar(255) DEFAULT NULL,
  `expires_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `revoked` tinyint(1) DEFAULT 0,
  `revoked_at` timestamp NULL DEFAULT NULL,
  `device_info` varchar(255) DEFAULT NULL,
  `ip_address` varchar(45) DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_user_sessions`
--

INSERT INTO `tb_user_sessions` (`session_id`, `user_id`, `refresh_token`, `token_family`, `expires_at`, `revoked`, `revoked_at`, `device_info`, `ip_address`, `created_at`, `updated_at`) VALUES
(1, 1, 'demo_refresh_token_1', 'family_1', '2026-04-19 04:33:21', 0, NULL, 'Chrome on Windows', '127.0.0.1', '2026-04-12 04:33:21', '2026-04-12 04:33:21'),
(2, 3, 'ZV7fQ3cLLQIkWmbr90BBXZp++1EBbLoF/QKH8Dk33eM=', '3a167a4f-1ce6-484e-aa0f-2f19ee09c3e5', '2026-06-13 21:25:02', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-14 21:25:02', '2026-05-15 04:25:02'),
(3, 3, 'GZgzXM52DKjObzB0THDxud/U5qixpp0x+ckjAFwnclc=', '0676dc84-7048-4435-ab63-5878066a47fc', '2026-06-14 09:43:41', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-15 09:43:41', '2026-05-15 16:43:41'),
(4, 3, 'GyKCNEmahYM72OCcKQcBsJWG8symyIaOGesOIr6zed4=', 'ff0fee19-b821-4a50-a1de-f0bb70b3ea77', '2026-06-14 09:54:39', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-15 09:54:39', '2026-05-15 16:54:39'),
(5, 3, 'QK/NnIIlXmGdfQhYKQwfwTpiUg/GpQ4Y0qniczc5aKA=', '73b08f65-422b-43d2-8b7a-fc5db47c2356', '2026-06-14 10:04:42', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-15 10:04:42', '2026-05-15 17:04:42'),
(6, 3, 'pRPcE/eTqOiXW14HspF44pC2rr0zi6zKUu3MvAFb11I=', 'ee02a7e7-2253-4d83-9013-086a1321c70c', '2026-06-14 10:33:39', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-15 10:33:39', '2026-05-15 17:33:40'),
(7, 3, '0AFpb2yFMuLoiucy4aIgKphVOzFfdRm30fFtlOPtCpo=', '65fa2c14-f409-4195-beb9-ddc19c3ca72f', '2026-06-14 23:19:03', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-15 23:19:03', '2026-05-16 06:19:03'),
(8, 3, 'H1BGBC94rq4TtVFQ/P2x3Zlwg8bMObdnukf8g0e0arQ=', '8218dd22-f20c-4420-865f-b28c002d04d4', '2026-06-14 23:29:03', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-15 23:29:03', '2026-05-16 06:29:03'),
(9, 7, 'Jni6jRx/nLul2wrip2l/vAdfz1XjqeFkCteRipIwa4M=', 'a17fd5cd-95f3-4bf4-985a-04b6dddbf5e5', '2026-06-15 00:59:28', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-16 00:59:28', '2026-05-16 07:59:28'),
(10, 8, 'PWcvB+kD9Q9IJGU0UbvuVPjIJ+jehKLOFFD6LT2AuDg=', 'db8651df-0468-4c43-b82b-6d38b1887fcf', '2026-06-15 01:00:31', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-16 01:00:31', '2026-05-16 08:00:31'),
(11, 8, 'Cj3T18KMNCwLmiO/TwThtSAT1dzp6XMN5X1CeQF+AYs=', '169aefe4-37eb-47d0-8534-df99dbab958b', '2026-06-15 01:06:31', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-16 01:06:31', '2026-05-16 08:06:31'),
(12, 3, 'iSGrUo6JNB/7Dk8Ki2MbSrai7dktsRhOKRsuaI9y1QU=', 'a4835f76-5ddb-4d3c-9aa5-5aef93dc8f3f', '2026-06-15 01:07:06', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-16 01:07:06', '2026-05-16 08:07:06'),
(13, 3, 'nJ+TybtFBpBxcfrFzfp56WWhRgyLAYQwwGaAEn81Xk4=', 'c25e0265-a8fc-4425-94a8-544dc8c289d2', '2026-06-15 01:12:53', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-16 01:12:53', '2026-05-16 08:12:53'),
(14, 3, 'ZAFeToR9+nKsV0B5h/9P2ZrhZSJl07yOlHuKdTG0bbE=', '4326341c-13ab-4e92-8410-b61c79755bd7', '2026-06-15 01:13:42', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-16 01:13:42', '2026-05-16 08:13:42'),
(15, 3, 'hmGjlJtDVPA8HG7OBJs8g5cZMWEWSxz9vwvWmBSWWZ4=', 'cedbcb0b-a221-45a8-99c0-07894b59f2bd', '2026-06-15 01:44:04', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-16 01:44:04', '2026-05-16 08:44:04'),
(16, 3, 'czwGWDz5354dwpYy0I7Ktu+jS1sJE837W6vLODJHTPI=', '7c0d0a51-22f8-4601-b5e0-b9fcd0daec29', '2026-06-15 02:47:31', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-16 02:47:31', '2026-05-16 09:47:31'),
(17, 7, '8D8fhIYJTjtJlRkMlutAX+NcjgKF2IU5wcbaVCUPE/U=', '1de20f23-14b4-4115-874a-f90a91a8ab7d', '2026-06-15 02:50:32', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-16 02:50:32', '2026-05-16 09:50:32'),
(18, 8, 'bricrZ1qa51W4vJ51DlH9//2BlwwLoA8lF3Hf0kR3v4=', 'ac08f8ec-386f-4d47-8aea-7826ab69fcde', '2026-06-15 02:50:45', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-16 02:50:45', '2026-05-16 09:50:45'),
(19, 3, 'ex4r7rUa6C4bgMxEnvy+9XySVn2RiBiBksYWRKoK7+U=', '5bce78bb-812f-4dc5-9e43-422c87082e92', '2026-06-15 02:55:55', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-16 02:55:55', '2026-05-16 09:55:55'),
(20, 9, 'verify_xQrMM7h+iJ6/r0sO/GKAw3mFjDrFyFhEoyw7S6HwMz8=', 'email_verification', '2026-05-17 02:57:01', 0, NULL, NULL, NULL, '2026-05-16 02:57:01', '2026-05-16 09:57:01'),
(21, 9, 'TfkdUTnhqDjAkUmy9ji0vAAJI1MqIEZNpdoaHPnINDA=', 'fc08d817-cf48-4118-8434-6fa719ca4cc2', '2026-06-15 03:01:09', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-16 03:01:09', '2026-05-16 10:01:09'),
(22, 9, 'Qe7+rycWd8+3/Tjb91w8+tCzmdwj2ULRKtJ9/M7P89U=', 'eec78f2e-f607-4658-95c1-26534a20d640', '2026-06-15 06:43:53', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '127.0.0.1', '2026-05-16 06:43:53', '2026-05-16 13:43:53'),
(23, 9, 'GJGiYwhg6ysVM0MVvGdkYCe/OCG2Pu68aBIZSYWTQsQ=', 'ebda5fae-8d45-462d-9bd9-47f95ba7499f', '2026-06-15 06:45:10', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '::1', '2026-05-16 06:45:10', '2026-05-16 13:45:10'),
(24, 3, 'J7fEQLcvNvUad+lyxUp4OfBd3NLb0O+Nh4KlU8wkaXk=', '1188f32e-9a64-4022-8b71-33ce326bcade', '2026-06-15 06:47:50', 0, NULL, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', '127.0.0.1', '2026-05-16 06:47:50', '2026-05-16 13:47:50');

-- --------------------------------------------------------

--
-- Table structure for table `tb_user_test_attempts`
--

CREATE TABLE `tb_user_test_attempts` (
  `attempt_id` int(11) NOT NULL,
  `user_id` int(11) DEFAULT NULL,
  `test_id` int(11) DEFAULT NULL,
  `started_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `finished_at` timestamp NULL DEFAULT NULL,
  `band_score` float DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_user_test_attempts`
--

INSERT INTO `tb_user_test_attempts` (`attempt_id`, `user_id`, `test_id`, `started_at`, `created_at`, `finished_at`, `band_score`) VALUES
(1, 1, 1, '2026-04-12 04:33:20', '2026-04-12 04:33:20', '2026-04-12 04:33:20', 6.5),
(2, 3, 1, '2026-04-12 03:32:22', '2026-04-12 10:32:22', '2026-04-12 03:32:22', 0.5),
(3, 9, 2, '2026-05-16 07:32:07', '2026-05-16 07:32:07', '2026-05-16 07:32:08', 0.5),
(4, 3, 601, '2026-05-20 19:30:58', '2026-05-20 19:30:58', '2026-05-20 19:30:58', 0.5),
(5, 3, 1, '2026-05-20 20:05:55', '2026-05-20 20:05:55', '2026-05-20 20:05:55', 0.5),
(6, 3, 2, '2026-05-20 20:18:30', '2026-05-20 20:18:30', '2026-05-20 20:18:30', 0.5);

-- --------------------------------------------------------

--
-- Table structure for table `tb_vocabulary`
--

CREATE TABLE `tb_vocabulary` (
  `vocab_id` int(11) NOT NULL,
  `word` varchar(255) DEFAULT NULL,
  `phonetic` varchar(100) DEFAULT NULL,
  `audio_url` varchar(500) DEFAULT NULL,
  `synonyms` text DEFAULT NULL,
  `antonyms` text DEFAULT NULL,
  `part_of_speech` varchar(50) DEFAULT NULL,
  `ielts_topic` varchar(100) DEFAULT NULL,
  `usage_frequency` int(11) DEFAULT NULL,
  `meaning` text DEFAULT NULL,
  `example` text DEFAULT NULL,
  `difficulty` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_vocabulary`
--

INSERT INTO `tb_vocabulary` (`vocab_id`, `word`, `phonetic`, `part_of_speech`, `ielts_topic`, `usage_frequency`, `meaning`, `example`, `difficulty`, `synonyms`, `antonyms`, `audio_url`) VALUES
(1, 'sustainable', '/səˈsteɪnəbl/', 'adjective', 'environment', 3, 'Bền vững, có thể duy trì lâu dài', 'Sustainable transport reduces carbon emissions.', 6, 'eco-friendly,viable', 'unsustainable', 'https://example.com/audio/sustainable.mp3'),
(2, 'biodiversity', '/ˌbaɪəʊdaɪˈvɜːsəti/', 'noun', 'environment', 3, 'Đa dạng sinh học', 'The forest has a rich biodiversity of plants and animals.', 7, 'variety,diversity', 'monoculture', 'https://example.com/audio/biodiversity.mp3'),
(3, 'mitigate', '/ˈmɪtɪɡeɪt/', 'verb', 'environment', 2, 'Giảm thiểu, làm nhẹ bớt', 'Governments must take action to mitigate the effects of global warming.', 8, 'alleviate,reduce', 'exacerbate,intensify', 'https://example.com/audio/mitigate.mp3'),
(4, 'unprecedented', '/ʌnˈpresɪdentɪd/', 'adjective', 'society', 2, 'Chưa từng có tiền lệ', 'The rapid growth of cities has caused unprecedented demands on public transport.', 8, 'exceptional,unparalleled', 'common,precedented', 'https://example.com/audio/unprecedented.mp3'),
(5, 'ubiquitous', '/juːˈbɪkwɪtəs/', 'adjective', 'technology', 3, 'Phổ biến, ở đâu cũng có', 'Smartphones have become ubiquitous in modern society.', 8, 'omnipresent,widespread', 'rare,scarce', 'https://example.com/audio/ubiquitous.mp3'),
(6, 'detrimental', '/ˌdetrɪˈmentl/', 'adjective', 'health', 3, 'Có hại, bất lợi', 'Lack of sleep has a detrimental effect on academic performance.', 7, 'harmful,damaging', 'beneficial,helpful', 'https://example.com/audio/detrimental.mp3'),
(7, 'exacerbate', '/ɪɡˈzæsəbeɪt/', 'verb', 'society', 2, 'Làm trầm trọng thêm', 'The economic crisis will exacerbate the housing shortage.', 8, 'worsen,aggravate', 'improve,alleviate', 'https://example.com/audio/exacerbate.mp3'),
(8, 'cognitive', '/ˈkɒɡnətɪv/', 'adjective', 'education', 3, 'Thuộc về nhận thức', 'Sleep is vital for children\'s cognitive development.', 7, 'mental,intellectual', 'physical', 'https://example.com/audio/cognitive.mp3'),
(9, 'pedagogical', '/ˌpedəˈɡɒdʒɪkl/', 'adjective', 'education', 2, 'Thuộc về sư phạm, giảng dạy', 'Teachers are adopting new pedagogical methods to engage students.', 8, 'educational,instructional', 'non-educational', 'https://example.com/audio/pedagogical.mp3'),
(10, 'curriculum', '/kəˈrɪkjələm/', 'noun', 'education', 3, 'Chương trình giảng dạy', 'The school curriculum includes both academic and vocational subjects.', 6, 'syllabus,course', 'extracurricular', 'https://example.com/audio/curriculum.mp3'),
(11, 'proliferation', '/prəˌlɪfəˈreɪʃn/', 'noun', 'technology', 2, 'Sự bùng nổ, sự tăng nhanh', 'The proliferation of social media has changed how people communicate.', 8, 'expansion,growth', 'reduction,decline', 'https://example.com/audio/proliferation.mp3'),
(12, 'obsolete', '/ˈɒbsəliːt/', 'adjective', 'technology', 2, 'Lỗi thời, không còn sử dụng', 'New technology quickly makes older computers obsolete.', 7, 'outdated,extinct', 'modern,current', 'https://example.com/audio/obsolete.mp3'),
(13, 'infrastructure', '/ˈɪnfrəstrʌktʃə(r)/', 'noun', 'economics', 3, 'Cơ sở hạ tầng', 'The government is investing in urban transport infrastructure.', 6, 'foundation,framework', 'superstructure', 'https://example.com/audio/infrastructure.mp3'),
(14, 'demographic', '/ˌdeməˈɡræfɪk/', 'noun', 'society', 2, 'Nhóm nhân khẩu học', 'The marketing campaign target is the young adult demographic.', 7, 'population sector,group', 'individual', 'https://example.com/audio/demographic.mp3'),
(15, 'socio-economic', '/ˌsəʊʃɪəʊ ˌiːkəˈnɒmɪk/', 'adjective', 'economics', 3, 'Thuộc về kinh tế - xã hội', 'Students from different socio-economic backgrounds study together.', 7, 'class-related,social', 'purely economic', 'https://example.com/audio/socio_economic.mp3'),
(16, 'equilibrium', '/ˌiːkwɪˈlɪbriəm/', 'noun', 'science', 2, 'Trạng thái cân bằng', 'Nature always seeks to maintain an ecological equilibrium.', 8, 'balance,stability', 'imbalance,instability', 'https://example.com/audio/equilibrium.mp3'),
(17, 'autonomous', '/ɔːˈtɒnəməs/', 'adjective', 'technology', 2, 'Tự trị, tự chủ, tự lái', 'Autonomous vehicles could significantly reduce traffic accidents.', 8, 'independent,self-governing', 'dependent,controlled', 'https://example.com/audio/autonomous.mp3'),
(18, 'depict', '/dɪˈpɪkt/', 'verb', 'culture', 3, 'Mô tả, khắc họa', 'The media often depicts teachers as academic leaders.', 7, 'portray,illustrate', 'misrepresent', 'https://example.com/audio/depict.mp3'),
(19, 'resilient', '/rɪˈzɪliənt/', 'adjective', 'society', 2, 'Kiên cường, phục hồi nhanh', 'Local communities proved highly resilient during the economic crisis.', 7, 'tough,adaptable', 'vulnerable,fragile', 'https://example.com/audio/resilient.mp3'),
(20, 'adversity', '/ədˈvɜːsəti/', 'noun', 'society', 2, 'Nghịch cảnh, hoàn cảnh khó khăn', 'He succeeded in university despite facing great personal adversity.', 8, 'hardship,misfortune', 'prosperity,benefit', 'https://example.com/audio/adversity.mp3'),
(21, 'disparity', '/dɪˈspærəti/', 'noun', 'economics', 2, 'Sự chênh lệch, sự bất bình đẳng', 'There is a growing disparity between urban and rural incomes.', 8, 'inequality,difference', 'similarity,equality', 'https://example.com/audio/disparity.mp3'),
(22, 'subsidize', '/ˈsʌbsɪdaɪz/', 'verb', 'economics', 2, 'Trợ cấp, bao cấp', 'The government subsidizes public transport to encourage its usage.', 7, 'fund,finance', 'tax,penalize', 'https://example.com/audio/subsidize.mp3'),
(23, 'indispensable', '/ˌɪndɪˈspensəbl/', 'adjective', 'society', 3, 'Không thể thiếu, thiết yếu', 'Digital devices have become indispensable tools for modern learning.', 7, 'essential,crucial', 'superfluous,redundant', 'https://example.com/audio/indispensable.mp3'),
(24, 'advocate', '/ˈædvəkeɪt/', 'verb', 'society', 3, 'Ủng hộ, tán thành', 'Many experts advocate adopting hybrid classes in universities.', 7, 'support,promote', 'oppose,criticize', 'https://example.com/audio/advocate.mp3'),
(25, 'conducive', '/kənˈdjuːsɪv/', 'adjective', 'education', 2, 'Có lợi, dẫn đến kết quả tốt', 'A quiet study room is highly conducive to learning.', 8, 'favorable,helpful', 'unfavorable,hindering', 'https://example.com/audio/conducive.mp3'),
(26, 'empirical', '/ɪmˈpɪrɪkl/', 'adjective', 'science', 2, 'Thực nghiệm, dựa trên thực tế', 'The research provides empirical evidence supporting sleep benefits.', 8, 'factual,observational', 'theoretical,speculative', 'https://example.com/audio/empirical.mp3'),
(27, 'facilitate', '/fəˈsɪlɪteɪt/', 'verb', 'education', 3, 'Tạo điều kiện, làm cho dễ dàng', 'Modern classroom software can facilitate student collaboration.', 7, 'ease,assist', 'hinder,impede', 'https://example.com/audio/facilitate.mp3'),
(28, 'holistic', '/həʊˈlɪstɪk/', 'adjective', 'health', 2, 'Toàn diện, tổng thể', 'Schools should adopt a holistic approach to student education.', 7, 'comprehensive,all-inclusive', 'narrow,fragmented', 'https://example.com/audio/holistic.mp3'),
(29, 'invaluable', '/ɪnˈvæljuəbl/', 'adjective', 'education', 3, 'Vô giá, cực kỳ hữu ích', 'Studying abroad provides students with invaluable life experience.', 7, 'priceless,precious', 'worthless,useless', 'https://example.com/audio/invaluable.mp3'),
(30, 'prevalent', '/ˈprevələnt/', 'adjective', 'health', 2, 'Phổ biến, thịnh hành', 'Sedentary lifestyles are highly prevalent in large cities.', 7, 'widespread,common', 'rare,unusual', 'https://example.com/audio/prevalent.mp3'),
(31, 'profound', '/prəˈfaʊnd/', 'adjective', 'society', 2, 'Sâu sắc, thâm thúy', 'The technological shift has had a profound impact on education.', 8, 'deep,intense', 'superficial,slight', 'https://example.com/audio/profound.mp3'),
(32, 'reconcile', '/ˈrekənsaɪl/', 'verb', 'society', 2, 'Hòa giải, làm cho nhất quán', 'It is difficult to reconcile industrial growth with environmental safety.', 8, 'harmonize,resolve', 'clash,disagree', 'https://example.com/audio/reconcile.mp3'),
(33, 'sedentary', '/ˈsedntri/', 'adjective', 'health', 3, 'Ít vận động, ngồi nhiều', 'A sedentary lifestyle is linked to heart disease and stress.', 7, 'inactive,sitting', 'active,mobile', 'https://example.com/audio/sedentary.mp3'),
(34, 'threshold', '/ˈθreʃhəʊld/', 'noun', 'science', 2, 'Ngưỡng, điểm bắt đầu', 'His score was just below the university admission threshold.', 7, 'boundary,limit', 'middle,end', 'https://example.com/audio/threshold.mp3'),
(35, 'viable', '/ˈvaɪəbl/', 'adjective', 'economics', 3, 'Khả thi, có thể phát triển', 'Converting waste to bricks is a financially viable project.', 7, 'feasible,workable', 'impossible,unviable', 'https://example.com/audio/viable.mp3'),
(36, 'counterpart', '/ˈkaʊntəpɑːt/', 'noun', 'society', 2, 'Đối tác, người đồng cấp', 'Students discuss with their counterparts in overseas schools.', 7, 'equivalent,peer', 'opponent', 'https://example.com/audio/counterpart.mp3'),
(37, 'vulnerable', '/ˈvʌlnərəbl/', 'adjective', 'society', 3, 'Dễ bị tổn thương, nguy hiểm', 'Low-income households are highly vulnerable to cost-of-living rises.', 7, 'susceptible,weak', 'immune,strong', 'https://example.com/audio/vulnerable.mp3'),
(38, 'utilize', '/ˈjuːtəlaɪz/', 'verb', 'general', 3, 'Sử dụng, tận dụng', 'Students must learn how to utilize library databases effectively.', 6, 'use,employ', 'waste,ignore', 'https://example.com/audio/utilize.mp3'),
(39, 'implement', '/ˈɪmplɪment/', 'verb', 'general', 3, 'Thực hiện, triển khai', 'The municipality decided to implement a new parking policy.', 6, 'execute,apply', 'cancel,neglect', 'https://example.com/audio/implement.mp3'),
(40, 'fluctuate', '/ˈflʌktʃueɪt/', 'verb', 'economics', 2, 'Dao động, biến động', 'Oil prices tend to fluctuate due to global supply disruptions.', 7, 'oscillate,vary', 'remain stable', 'https://example.com/audio/fluctuate.mp3'),
(41, 'enhance', '/ɪnˈhɑːns/', 'verb', 'general', 3, 'Nâng cao, cải thiện', 'Reading regularly is the best way to enhance your vocabulary.', 6, 'improve,boost', 'degrade,damage', 'https://example.com/audio/enhance.mp3'),
(42, 'acquire', '/əˈkwaɪə(r)/', 'verb', 'general', 3, 'Đạt được, thu nhận', 'Living in a foreign country helps you acquire language skills naturally.', 6, 'obtain,gain', 'lose,forfeit', 'https://example.com/audio/acquire.mp3');

-- --------------------------------------------------------

--
-- Table structure for table `tb_writing_prompts`
--

CREATE TABLE `tb_writing_prompts` (
  `prompt_id` int(11) NOT NULL,
  `task_type` enum('task1','task2') NOT NULL,
  `prompt_text` text NOT NULL,
  `prompt_image_url` varchar(500) DEFAULT NULL,
  `chart_type` varchar(50) DEFAULT NULL,
  `difficulty_level` int(11) DEFAULT 1,
  `category` varchar(100) DEFAULT NULL,
  `band_target` int(11) DEFAULT 6,
  `sample_answer` text DEFAULT NULL,
  `notes_for_teacher` text DEFAULT NULL,
  `is_active` tinyint(1) DEFAULT 1,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `status` enum('draft','pending_review','published','rejected') NOT NULL DEFAULT 'draft',
  `created_by` int(11) DEFAULT NULL,
  `reviewed_by` int(11) DEFAULT NULL,
  `reviewed_at` datetime DEFAULT NULL,
  `reviewer_note` text DEFAULT NULL,
  `is_deleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_writing_prompts`
--

INSERT INTO `tb_writing_prompts` (`prompt_id`, `task_type`, `prompt_text`, `prompt_image_url`, `chart_type`, `difficulty_level`, `category`, `band_target`, `sample_answer`, `notes_for_teacher`, `is_active`, `created_at`, `updated_at`, `status`, `created_by`, `reviewed_by`, `reviewed_at`, `reviewer_note`, `is_deleted`) VALUES
(1, 'task1', 'The chart below shows energy usage by sector from 2000 to 2020.', '/uploads/writing/0a040568-d58f-4606-9c4b-dbc6bd39fac4.png', 'line', 2, 'energy', 6, 'Sample response text...', 'Ask student to compare trends.', 1, '2026-04-12 04:33:21', '2026-05-16 15:48:53', 'published', NULL, 8, '2026-05-16 15:48:53', NULL, 0),
(1401, 'task1', 'The bar chart compares the proportion of household expenditure on transport, food, and housing in three cities in 2010 and 2020. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', NULL, 'bar', 3, 'urban economics', 6, 'A strong response identifies highest and lowest categories, highlights major shifts, and avoids unsupported causes.', 'Check overview quality and data grouping logic.', 1, '2026-04-12 09:42:58', '2026-05-16 08:01:04', 'rejected', NULL, NULL, NULL, NULL, 1),
(1402, 'task1', 'The line graph shows the percentage of commuters using four transport modes between 2005 and 2025 in a major city. Summarise the information by selecting and reporting the main features.', '/uploads/writing/1e2033e9-2304-46df-b7f5-e4f9a358208d.png', 'line', 3, 'transport', 6, 'Band 7+ response shows trend language, key comparisons, and selective data reporting.', 'Watch tense consistency and comparative structures.', 1, '2026-04-12 09:42:58', '2026-05-16 02:15:11', 'draft', NULL, NULL, NULL, NULL, 0),
(1403, 'task1', 'The diagram illustrates the process of municipal waste recycling into construction materials. Summarise the process by selecting and reporting the main stages.', NULL, 'process', 3, 'environment', 6, 'Good response uses passive voice and chronological linking across stages.', 'Check sequence control and process vocabulary.', 1, '2026-04-12 09:42:58', '2026-04-12 09:42:58', 'draft', NULL, NULL, NULL, NULL, 0),
(1404, 'task1', 'The maps show a town center in 2000 and 2025 after redevelopment. Summarise the changes by selecting and reporting key features.', NULL, 'map', 3, 'urban planning', 6, 'High-band answer clusters changes by area and purpose rather than listing randomly.', 'Check spatial grouping and overview sentence.', 1, '2026-04-12 09:42:58', '2026-04-12 09:42:58', 'draft', NULL, NULL, NULL, NULL, 0),
(1405, 'task2', 'Some people believe remote work is beneficial for employees and employers, while others think office-based work is more effective. Discuss both views and give your own opinion.', NULL, NULL, 3, 'work and society', 6, 'A balanced essay considers productivity, collaboration, and long-term career development before giving a clear stance.', 'Evaluate thesis clarity and paragraph unity.', 1, '2026-04-12 09:42:58', '2026-04-12 09:42:58', 'draft', NULL, NULL, NULL, NULL, 0),
(1406, 'task2', 'In many cities, traffic congestion is worsening. What are the main causes of this problem, and what measures can governments and individuals take to solve it?', NULL, NULL, 3, 'transport policy', 6, 'Band 7+ writing explains multiple causes and realistic multi-level solutions.', 'Check relevance and feasibility of solutions.', 1, '2026-04-12 09:42:58', '2026-04-12 09:42:58', 'draft', NULL, NULL, NULL, NULL, 0),
(1407, 'task2', 'Some people think university education should be free for everyone. To what extent do you agree or disagree?', NULL, NULL, 3, 'education policy', 6, 'High-quality response weighs social equity against fiscal sustainability and states a consistent position.', 'Check argument development and support.', 1, '2026-04-12 09:42:58', '2026-04-12 09:42:58', 'draft', NULL, NULL, NULL, NULL, 0),
(1408, 'task2', 'Many believe technological progress improves quality of life, while others argue it creates new social problems. Discuss both views and give your opinion.', NULL, NULL, 3, 'technology and society', 6, 'Strong essays compare benefits and unintended effects with concrete examples.', 'Check coherence and lexical precision.', 1, '2026-04-12 09:42:58', '2026-04-12 09:42:58', 'draft', NULL, NULL, NULL, NULL, 0),
(1409, 'task2', 'Some countries invest heavily in public transport, whereas others prioritize road expansion for private vehicles. Discuss both approaches and give your opinion.', NULL, NULL, 4, 'infrastructure', 7, 'Band 7 target requires nuanced comparison and clear policy evaluation.', 'Assess critical reasoning depth.', 1, '2026-04-12 09:42:58', '2026-04-12 09:42:58', 'draft', NULL, NULL, NULL, NULL, 0),
(1410, 'task2', 'The gap between rich and poor is increasing in many societies. What problems can this cause, and what practical solutions are available?', NULL, NULL, 4, 'social policy', 7, 'Top responses move beyond generic claims and provide actionable interventions.', 'Check specificity and evidence quality.', 1, '2026-04-12 09:42:58', '2026-04-12 09:42:58', 'draft', NULL, NULL, NULL, NULL, 0),
(2001, 'task1', 'The bar chart below shows the percentage of people in three age groups (18-30, 31-50, 51+) who used the internet daily in four countries in 2023. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', NULL, 'bar', 3, 'technology', 7, 'The bar chart compares daily internet usage across three age groups in four countries in 2023. Overall, younger people used the internet more frequently in all nations, though the gap between age groups varied significantly by country.\n\nIn Country A and Country B, the 18-30 age group had the highest usage at 95% and 91% respectively, while the over-51 group lagged behind at 62% and 58%. Country C showed the most even distribution, with all three groups falling between 70% and 82%. Country D had the widest gap: 93% of young adults used the internet daily compared to just 41% of those aged 51 and over.\n\nIn summary, while internet adoption was high among young adults everywhere, digital engagement among older populations varied considerably between nations.', 'Check overview quality, data selection, and comparative language.', 1, '2026-05-16 15:45:17', '2026-05-16 15:45:17', 'published', NULL, NULL, NULL, NULL, 0),
(2002, 'task1', 'The two maps below show a small coastal town in 1990 and the same town in 2025. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', NULL, 'map', 3, 'urban development', 7, 'The maps illustrate changes to a coastal town between 1990 and 2025. Overall, the town underwent substantial modernisation, with industrial areas replaced by leisure facilities and improved transport links.\n\nIn 1990 the northern area contained a fish processing factory and warehouse, while the southern coastline had a small beach and a row of fishermen\'s cottages. By 2025, the factory site had been redeveloped into a hotel complex with a swimming pool, and the warehouse was converted into a shopping centre. The cottages were demolished and replaced by a marina with boat moorings.\n\nA new coastal road was built connecting the eastern residential area to the marina, and a car park was added near the hotel. The only feature that remained unchanged was the church in the town centre.', 'Evaluate spatial grouping and use of change vocabulary.', 1, '2026-05-16 15:45:17', '2026-05-16 15:45:17', 'published', NULL, NULL, NULL, NULL, 0),
(2003, 'task2', 'Some people believe that children should begin learning a foreign language in primary school rather than in secondary school. To what extent do you agree or disagree?', NULL, NULL, 3, 'education', 7, 'Starting foreign language instruction in primary school offers clear cognitive and practical advantages, and I strongly agree that early introduction is preferable.\n\nYoung children possess a natural capacity for language acquisition. Research shows that the brain\'s neural plasticity peaks before age ten, making pronunciation and grammar patterns easier to absorb. Children who begin bilingual education at age six typically achieve near-native fluency by their mid-teens, whereas those who start at twelve rarely reach equivalent pronunciation accuracy.\n\nFurthermore, early language learning develops broader cognitive skills. Studies have linked childhood bilingualism to improved executive function, enhanced problem-solving ability, and greater cultural empathy. These benefits compound over time and are difficult to replicate through later instruction alone.\n\nCritics argue that primary curricula are already crowded and that children should first master their mother tongue. However, evidence from countries such as the Netherlands and Singapore, where multilingual education begins at age five, shows no negative effect on first-language proficiency.\n\nIn conclusion, the cognitive window for language learning is limited, and delaying instruction to secondary school wastes a valuable developmental opportunity.', 'Check thesis clarity, argument depth, and conclusion consistency.', 1, '2026-05-16 15:45:17', '2026-05-16 15:45:17', 'published', NULL, NULL, NULL, NULL, 0),
(2004, 'task2', 'In many countries, the gap between the cost of living and average wages is growing. What problems does this cause, and what measures could governments take to address the issue?', NULL, NULL, 3, 'economics', 7, 'The widening gap between living costs and wages creates serious social problems that require coordinated government intervention.\n\nThe most immediate consequence is housing insecurity. When rent or mortgage payments consume more than forty percent of household income, families are forced to sacrifice spending on nutrition, healthcare and education. This in turn creates a cycle of disadvantage that is difficult to escape.\n\nA second problem is reduced consumer spending. When disposable income shrinks, retail and service sectors contract, leading to job losses and slower economic growth. The resulting anxiety can also affect mental health, with studies linking financial stress to higher rates of depression.\n\nGovernments can respond in several ways. First, raising the minimum wage to a genuine living wage ensures that full-time workers can meet basic needs. Second, investing in affordable public housing reduces the largest single expense for low-income households. Third, targeted subsidies for childcare and transport can free up income for other essentials.\n\nIn conclusion, the cost-of-living crisis demands practical, multi-level policy responses rather than reliance on market self-correction alone.', 'Check cause-solution structure and feasibility of proposals.', 1, '2026-05-16 15:45:17', '2026-05-16 15:45:17', 'published', NULL, NULL, NULL, NULL, 0),
(2005, 'task1', 'a', NULL, NULL, 1, NULL, 6, 'a', NULL, 1, '2026-05-20 09:45:17', '2026-05-20 16:45:23', 'rejected', 8, NULL, NULL, NULL, 1);

-- --------------------------------------------------------

--
-- Table structure for table `tb_writing_submissions`
--

CREATE TABLE `tb_writing_submissions` (
  `submission_id` int(11) NOT NULL,
  `user_id` int(11) DEFAULT NULL,
  `prompt` text DEFAULT NULL,
  `essay_text` text DEFAULT NULL,
  `band_score` float DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `ai_feedback` text DEFAULT NULL,
  `ta_score` decimal(2,1) DEFAULT 0.0 COMMENT 'Task Response',
  `cc_score` decimal(2,1) DEFAULT 0.0 COMMENT 'Coherence and Cohesion',
  `lr_score` decimal(2,1) DEFAULT 0.0 COMMENT 'Lexical Resource',
  `gra_score` decimal(2,1) DEFAULT 0.0 COMMENT 'Grammatical Range and Accuracy',
  `feedback_details` text DEFAULT NULL COMMENT 'Chi tiết nhận xét từ AI cho từng tiêu chí',
  `word_count` int(11) DEFAULT NULL,
  `task_type` int(11) DEFAULT 2
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `tb_writing_submissions`
--

INSERT INTO `tb_writing_submissions` (`submission_id`, `user_id`, `prompt`, `essay_text`, `band_score`, `created_at`, `ai_feedback`, `ta_score`, `cc_score`, `lr_score`, `gra_score`, `feedback_details`, `word_count`, `task_type`) VALUES
(1, 1, 'Task 2: Public transport should be free. Discuss both views.', 'In this essay, I will discuss both perspectives...', 6.5, '2026-04-12 04:33:21', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(2, 3, 'The line graph shows the percentage of commuters using four transport modes between 2005 and 2025 in a major city. Summarise the information by selecting and reporting the main features.', 'Model Report: Town Center Transformation (2000 vs. 2025)\nThe two maps illustrate the structural alterations in a town center over a twenty-five-year period, from 2000 to 2025. Overall, the town underwent a significant transformation from a residential and industrial-focused area to one centered on commercial leisure and improved infrastructure.\n\nInfrastructure and Pedestrianization\nA major change is the pedestrianization of the main thoroughfare. In 2000, a main road ran through the center, accessible to cars. By 2025, this has been converted into a pedestrian-only zone, with the addition of a new ring road encircling the town center to divert traffic. Furthermore, the old bus station has been modernized and expanded to include a light rail link, enhancing public transport connectivity.\n\nCommercial and Residential Shifts\nThe landscape of buildings has also shifted. The following changes are notable:\n\nDemolition: The old factories located in the southeast corner were demolished and replaced by a large shopping mall.\n\nConversion: Several terraced houses to the west were converted into trendy apartments and a multi-story car park.\n\nGreen Space: A small park that existed in 2000 has been extended, now featuring a new fountain and a children’s play area.\n\nConclusion\nIn conclusion, the town center has become more modernized and accessible. The replacement of industrial zones with commercial hubs and the shift toward a car-free center are the most prominent features of the 2025 redevelopment plan.', 6, '2026-04-12 20:07:20', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(3, 3, 'The gap between rich and poor is increasing in many societies. What problems can this cause, and what practical solutions are available?', 'The consequences of a significant wealth gap extend beyond mere financial statistics, impacting social cohesion and public health.\n\nSocial Instability and Crime: High levels of inequality often lead to a sense of injustice and frustration among the lower classes. Research suggests a strong correlation between wealth disparity and increased rates of property crime and civil unrest, as individuals feel the \"system\" is rigged against them.\n\nReduced Social Mobility: When wealth is concentrated at the top, access to high-quality education and networking becomes a luxury. This creates a cycle where the poor remain poor regardless of their talent or effort, stifling the overall economic potential of a nation.\n\nPublic Health Crisis: Wealth inequality is often linked to poorer health outcomes. The \"status anxiety\" caused by a massive gap can lead to higher rates of stress-related illnesses, while the poor may lack the resources to afford preventative healthcare or nutritious food.\n\nPractical Solutions\nAddressing this issue requires a multi-faceted approach involving both legislative reform and social investment.\n\nProgressive Taxation: Governments can implement or strengthen progressive tax systems where higher earners contribute a larger percentage of their income. This revenue can then be redistributed into public services.\n\nInvestment in Education and Vocational Training: To break the cycle of poverty, governments must ensure that quality education is a right, not a privilege. Subsidizing university tuition or investing in trade schools allows individuals from low-income backgrounds to compete in the modern job market.\n\nImplementing a Living Wage: Raising the minimum wage to a \"living wage\" ensures that full-time workers can afford basic necessities. This reduces the reliance on state welfare and boosts the purchasing power of the lower and middle classes, which in turn stimulates the economy.\n\nConclusion\nIn conclusion, while the increasing gap between the rich and the poor poses significant threats to social order and individual well-being, it is not an unsolvable problem. Through equitable taxation, educational empowerment, and fair labor practices, societies can begin to bridge this divide. A more balanced distribution of wealth is not just a matter of charity; it is a fundamental requirement for a stable and prosperous future.', 6, '2026-04-12 20:08:57', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(4, 3, 'The maps show a town center in 2000 and 2025 after redevelopment. Summarise the changes by selecting and reporting key features.', 'Problems Caused by Wealth Inequality\nThe consequences of a significant wealth gap extend beyond mere financial statistics, impacting social cohesion and public health.\n\nSocial Instability and Crime: High levels of inequality often lead to a sense of injustice and frustration among the lower classes. Research suggests a strong correlation between wealth disparity and increased rates of property crime and civil unrest, as individuals feel the \"system\" is rigged against them.\n\nReduced Social Mobility: When wealth is concentrated at the top, access to high-quality education and networking becomes a luxury. This creates a cycle where the poor remain poor regardless of their talent or effort, stifling the overall economic potential of a nation.\n\nPublic Health Crisis: Wealth inequality is often linked to poorer health outcomes. The \"status anxiety\" caused by a massive gap can lead to higher rates of stress-related illnesses, while the poor may lack the resources to afford preventative healthcare or nutritious food.\n\nPractical Solutions\nAddressing this issue requires a multi-faceted approach involving both legislative reform and social investment.\n\nProgressive Taxation: Governments can implement or strengthen progressive tax systems where higher earners contribute a larger percentage of their income. This revenue can then be redistributed into public services.\n\nInvestment in Education and Vocational Training: To break the cycle of poverty, governments must ensure that quality education is a right, not a privilege. Subsidizing university tuition or investing in trade schools allows individuals from low-income backgrounds to compete in the modern job market.\n\nImplementing a Living Wage: Raising the minimum wage to a \"living wage\" ensures that full-time workers can afford basic necessities. This reduces the reliance on state welfare and boosts the purchasing power of the lower and middle classes, which in turn stimulates the economy.\n\nConclusion\nIn conclusion, while the increasing gap between the rich and the poor poses significant threats to social order and individual well-being, it is not an unsolvable problem. Through equitable taxation, educational empowerment, and fair labor practices, societies can begin to bridge this divide. A more balanced distribution of wealth is not just a matter of charity; it is a fundamental requirement for a stable and prosperous future.', 6.7, '2026-04-13 03:09:22', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(5, 3, 'The line graph shows the percentage of commuters using four transport modes between 2005 and 2025 in a major city. Summarise the information by selecting and reporting the main features.', 'Chào bạn! Rất sẵn lòng hỗ trợ bạn với bài viết này. Tuy nhiên, có một lưu ý nhỏ: yêu cầu của bạn là tóm tắt quy trình từ biểu đồ, đây thực chất là dạng bài IELTS Writing Task 1 (mô tả biểu đồ), chứ không phải Task 2 (viết bài luận nghị luận).\n\nDưới đây là bản tóm tắt quy trình chuyển hóa rác thải đô thị thành vật liệu xây dựng theo đúng tiêu chuẩn học thuật của IELTS Writing Task 1.\n\nIELTS Writing Task 1: Municipal Waste Recycling Process\nIntroduction & Overview\nThe diagram delineates the industrial stages involved in transforming municipal waste into various materials for the construction industry.\n\nOverall, the process is a linear sequence beginning with waste collection and ending with the production of specialized building components. It consists of three primary stages: initial sorting, high-temperature processing, and the final manufacturing of bricks and road-surfacing materials.\n\nStage 1: Sorting and Pre-treatment\nThe process commences when municipal waste is gathered and put through a sorting machine. At this stage, the waste is categorized, and any components unsuitable for construction—such as organic matter or certain recyclables—are filtered out. The remaining raw waste is then transported to a crushing machine, where it is broken down into smaller, uniform pieces to facilitate the subsequent chemical treatments.\n\nStage 2: Thermal Processing (Innoculation & Incineration)\nOnce crushed, the material undergoes a two-step thermal treatment:\n\nInnoculation: The waste is placed in an innoculation device where it is treated under controlled conditions.\n\nIncineration: Following treatment, the material is moved to a furnace for incineration at extremely high temperatures. This combustion process results in two distinct outputs: waste gas (which is released or treated) and ash.\n\nStage 3: Final Production\nIn the concluding stage, the recovered ash serves as the primary raw material for construction products.\n\nThe ash is mixed with water in a dedicated chamber to create a workable mixture.\n\nThis mixture is then molded and processed to produce bricks.\n\nSimultaneously, the heavier residues or processed outputs are utilized as road-surfacing materials, completing the recycling cycle.', 7.1, '2026-04-14 02:43:55', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(6, 3, 'The bar chart compares the proportion of household expenditure on transport, food, and housing in three cities in 2010 and 2020. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', 'Dưới đây là bài mẫu hoàn chỉnh cho đề bài IELTS Writing Task 1 (Bar Chart) về chi tiêu hộ gia đình, được biên soạn theo phong cách học thuật giúp bạn tối ưu điểm số:\n\nIELTS Writing Task 1 Sample Answer\nThe bar chart illustrates the proportion of national income spent by households on three categories—transport, food, and housing—in three different cities in 2010 and 2020.\n\nOverall, housing remained the most significant expense for residents in all three cities throughout the ten-year period. Additionally, while the share of spending on transport and housing saw an upward trend, the figures for food experienced a moderate decline across the board.\n\nIn terms of housing and food, City C recorded the highest expenditure on housing, starting at 40% in 2010 and rising to 45% by 2020. Similarly, City A saw an increase in housing costs from 35% to 40%. Conversely, the percentage of spending on food followed a downward trajectory. For instance, in City C, the allocation for food dropped from 35% to 30%, a trend mirrored in both City A and City B, where residents spent approximately 5% less on food over the decade.\n\nRegarding transport, this category accounted for the smallest portion of household budgets but showed consistent growth. City B had the highest transport expenditure, reaching 25% in 2020. Meanwhile, City A experienced the most notable relative increase, with its transport spending climbing from 10% in 2010 to 15% in 2020. In City C, the figure for transport also grew steadily, ending the period at 20%.', 6.2, '2026-05-12 02:01:56', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(7, 3, 'The bar chart compares the proportion of household expenditure on transport, food, and housing in three cities in 2010 and 2020. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', 'The bar chart illustrates the percentage of household expenditure on transport, food, and housing in three cities between 2010 and 2020. Overall, housing accounted for the largest proportion of spending in both years, while the percentage spent on transport increased over time. In contrast, expenditure on food showed a downward trend.\n\nIn 2010, households spent around 45% of their budget on housing, making it the highest category. Food ranked second at approximately 35%, whereas transport represented the smallest share at only 20%. By 2020, spending on transport had risen significantly to 30%, indicating that transportation became more important in household budgets. Meanwhile, the proportion allocated to food decreased to 25%. Housing expenditure remained stable at about 45%, continuing to be the largest expense in all three cities.\n\nOverall, the chart highlights a shift in spending habits, with people allocating more money to transport and less to food over the ten-year period.', 7.2, '2026-05-18 02:15:12', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(8, 3, 'The bar chart compares the proportion of household expenditure on transport, food, and housing in three cities in 2010 and 2020. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', 'The bar chart provides information about the proportion of household spending on transport, food, and housing in three different cities in 2010 and 2020. Overall, housing remained the largest expense throughout the period, while spending on transport experienced a noticeable increase. By contrast, the percentage of money allocated to food declined over the ten years.\n\nIn 2010, housing accounted for nearly half of total household expenditure, at around 45%. Food was the second biggest category, representing approximately 35% of spending, whereas transport made up only 20%, the smallest share among the three categories. However, by 2020, there had been a significant change in spending patterns. Expenditure on transport rose considerably to 30%, suggesting that transportation costs became more important in people’s daily lives. Meanwhile, spending on food fell to about 25%. Despite these changes, the proportion spent on housing stayed stable at roughly 45%, continuing to dominate household budgets in all three cities.\n\nIn conclusion, the chart demonstrates a clear shift in household expenditure over the decade, with transport costs increasing steadily while food expenses became less significant. Housing, however, consistently remained the primary area of spending.', 7.2, '2026-05-18 02:16:27', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(9, 3, 'The bar chart compares the proportion of household expenditure on transport, food, and housing in three cities in 2010 and 2020. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', 'The bar chart illustrates the proportions of household expenditure allocated to transport, food, and housing in three cities in 2010 and 2020. Overall, housing consistently accounted for the largest share of spending in both years, whereas transport experienced the most significant growth over the period. In contrast, the proportion of expenditure on food declined noticeably.\n\nIn 2010, housing represented the greatest percentage of household budgets, at approximately 45%. Food was the second-largest category, accounting for around 35% of total expenditure, while transport made up the smallest proportion, at only 20%. This indicates that the majority of household income was primarily devoted to accommodation and daily necessities at the beginning of the period.\n\nBy 2020, however, spending patterns had changed considerably. The proportion allocated to transport rose sharply to 30%, reflecting the increasing importance of mobility and commuting costs in urban lifestyles. Meanwhile, expenditure on food fell substantially to 25%, suggesting that households may have become more efficient in managing food-related expenses or shifted their priorities toward other areas. Despite these fluctuations, housing expenditure remained unchanged at roughly 45%, continuing to dominate household budgets across the three cities.\n\nIn summary, the chart highlights a clear transformation in household spending habits between 2010 and 2020. While housing maintained its position as the most significant expense, transport became increasingly costly, whereas food gradually accounted for a smaller share of overall expenditure.', 7.4, '2026-05-18 02:17:52', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(10, 3, 'The bar chart compares the proportion of household expenditure on transport, food, and housing in three cities in 2010 and 2020. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', 'The bar chart illustrates the proportions of household expenditure devoted to transport, food, and housing in three cities in 2010 and 2020. Overall, housing consistently represented the largest share of spending in both years, while transport experienced a notable increase over the decade. By contrast, expenditure on food declined considerably during the same period.\n\nIn 2010, households allocated the majority of their budgets to housing, which accounted for approximately 45% of total expenditure. Food was the second-largest category at around 35%, whereas transport represented the smallest proportion, at only 20%. This distribution suggests that accommodation and daily necessities were the primary financial priorities for residents at the beginning of the period.\n\nBy 2020, however, spending habits had shifted noticeably. The proportion of expenditure on transport rose significantly to 30%, indicating that transportation costs became increasingly important in urban living. In contrast, the percentage spent on food fell to roughly 25%, reflecting a reduced share of household budgets. Despite these changes, housing expenditure remained stable at about 45%, continuing to dominate overall spending across the three cities.\n\nIn summary, the chart highlights a clear change in household spending patterns between 2010 and 2020. Although housing remained the most substantial expense throughout the period, transport gained greater financial importance, while food accounted for a smaller proportion of total household expenditure.', 7, '2026-05-18 02:18:49', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(11, 3, 'The bar chart compares the proportion of household expenditure on transport, food, and housing in three cities in 2010 and 2020. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', 'The bar chart compares the proportions of household expenditure allocated to transport, food, and housing in three cities in 2010 and 2020. Overall, housing consistently accounted for the largest share of spending throughout the period, whereas transport experienced a considerable rise. In contrast, the proportion of income devoted to food declined noticeably over the decade.\n\nIn 2010, housing represented approximately 45% of total household expenditure, making it by far the most dominant category in all three cities. Food ranked second, accounting for around 35% of spending, while transport constituted the smallest proportion at just 20%. This pattern suggests that residents primarily prioritized accommodation and essential living expenses at the beginning of the period.\n\nA decade later, significant changes could be observed in household spending habits. The proportion allocated to transport increased markedly to 30%, which may reflect rising fuel prices, greater dependence on private vehicles, or the expansion of urban commuting systems. Meanwhile, expenditure on food fell substantially to 25%, indicating that households devoted a smaller fraction of their budgets to daily consumption than before. Despite these fluctuations, housing expenditure remained stable at roughly 45%, demonstrating that accommodation continued to be the greatest financial burden for urban residents.\n\nOverall, the chart reveals a gradual shift in consumer priorities between 2010 and 2020. While housing maintained its position as the largest area of expenditure, transport became increasingly significant, whereas food occupied a less prominent share of household budgets across the three cities.', 7.3, '2026-05-18 02:19:40', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(12, 3, 'The bar chart compares the proportion of household expenditure on transport, food, and housing in three cities in 2010 and 2020. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', '“Sometimes I seriously wonder whether AI is genuinely intelligent or simply pretending to be. It often produces long, sophisticated answers filled with advanced vocabulary, yet somehow still manages to completely miss the point. You ask one simple question, and it responds with an unnecessary essay that sounds impressive but lacks real understanding. What makes it even funnier is the confidence with which AI delivers incorrect information, as if being wrong loudly somehow makes it right. Despite all the hype surrounding artificial intelligence, there are moments when it behaves less like a revolutionary technology and more like a student who memorized an entire textbook without actually understanding a single page of it.', 6.8, '2026-05-18 02:20:44', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(13, 3, 'The chart below shows energy usage by sector from 2000 to 2020.', 'The line chart illustrates the amount of energy consumed by different sectors between 2000 and 2020.\n\nOverall, energy usage increased in most sectors over the period shown. The industrial sector consistently consumed the largest amount of energy, while the residential sector used the least.\n\nIn 2000, the industrial sector accounted for approximately 40 units of energy consumption. This figure rose steadily and reached around 55 units by 2020. Similarly, the transportation sector experienced significant growth, climbing from about 25 units in 2000 to nearly 45 units at the end of the period.\n\nMeanwhile, the commercial sector showed a moderate upward trend. Energy consumption in this sector increased gradually from roughly 15 units to around 30 units over the twenty-year period. In contrast, the residential sector remained comparatively low, although it still recorded a slight rise from about 10 units in 2000 to approximately 20 units in 2020.\n\nIn conclusion, all sectors witnessed increases in energy usage, with the industrial and transportation sectors showing the most notable growth.', 7.2, '2026-05-19 06:42:41', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(14, 3, 'The bar chart below shows the percentage of people in three age groups (18-30, 31-50, 51+) who used the internet daily in four countries in 2023. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', 'The line chart illustrates the amount of energy consumed by different sectors between 2000 and 2020.\nOverall, energy usage increased in most sectors over the period shown. The industrial sector consistently consumed the largest amount of energy, while the residential sector used the least.\nIn 2000, the industrial sector accounted for approximately 40 units of energy consumption. This figure rose steadily and reached around 55 units by 2020. Similarly, the transportation sector experienced significant growth, climbing from about 25 units in 2000 to nearly 45 units at the end of the period.\nMeanwhile, the commercial sector showed a moderate upward trend. Energy consumption in this sector increased gradually from roughly 15 units to around 30 units over the twenty-year period. In contrast, the residential sector remained comparatively low, although it still recorded a slight rise from about 10 units in 2000 to approximately 20 units in 2020.\nIn conclusion, all sectors witnessed increases in energy usage, with the industrial and transportation sectors showing the most notable growth.', 6.6, '2026-05-19 08:40:28', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(15, 3, 'The two maps below show a small coastal town in 1990 and the same town in 2025. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', 'The two maps illustrate the changes that took place in a small coastal town between 1990 and 2025.\nOverall, the town experienced significant development over the 35-year period. Many new facilities were constructed, including residential areas, tourist attractions, and improved transport infrastructure, while some natural and undeveloped areas disappeared.\nIn 1990, the town was relatively small and simple in structure. A fishing harbour was located on the eastern coast, while a small beach lay to the south. There were only a few houses situated near the main road, and the western side of the town consisted mainly of farmland and woodland. In addition, there was a small local market in the town centre.\nBy 2025, the town had become far more urbanised and tourism-oriented. The fishing harbour had been transformed into a marina for leisure boats, and a large hotel was built near the beach to attract visitors. Several new residential buildings and apartments were constructed, particularly in the western area where farmland once existed. The woodland was partly cleared to make space for a shopping centre and a car park.\nTransportation infrastructure was also upgraded considerably. The main road was widened, and a new bridge connected the two sides of the town more efficiently. Furthermore, more public facilities such as restaurants and entertainment areas appeared along the coastline, reflecting the town’s shift towards tourism and modern urban living.', 6, '2026-05-19 20:01:16', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(16, 3, 'The chart below shows energy usage by sector from 2000 to 2020.', 'The two maps illustrate the changes that took place in a small coastal town between 1990 and 2025.\nOverall, the town experienced significant development over the 35-year period. Many new facilities were constructed, including residential areas, tourist attractions, and improved transport infrastructure, while some natural and undeveloped areas disappeared.\nIn 1990, the town was relatively small and simple in structure. A fishing harbour was located on the eastern coast, while a small beach lay to the south. There were only a few houses situated near the main road, and the western side of the town consisted mainly of farmland and woodland. In addition, there was a small local market in the town centre.\nBy 2025, the town had become far more urbanised and tourism-oriented. The fishing harbour had been transformed into a marina for leisure boats, and a large hotel was built near the beach to attract visitors. Several new residential buildings and apartments were constructed, particularly in the western area where farmland once existed. The woodland was partly cleared to make space for a shopping centre and a car park.\nTransportation infrastructure was also upgraded considerably. The main road was widened, and a new bridge connected the two sides of the town more efficiently. Furthermore, more public facilities such as restaurants and entertainment areas appeared along the coastline, reflecting the town’s shift towards tourism and modern urban living.', 6, '2026-05-19 20:10:16', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(17, 3, 'The chart below shows energy usage by sector from 2000 to 2020.', 'The two maps illustrate the changes that took place in a small coastal town between 1990 and 2025.\nOverall, the town experienced significant development over the 35-year period. Many new facilities were constructed, including residential areas, tourist attractions, and improved transport infrastructure, while some natural and undeveloped areas disappeared.\nIn 1990, the town was relatively small and simple in structure. A fishing harbour was located on the eastern coast, while a small beach lay to the south. There were only a few houses situated near the main road, and the western side of the town consisted mainly of farmland and woodland. In addition, there was a small local market in the town centre.\nBy 2025, the town had become far more urbanised and tourism-oriented. The fishing harbour had been transformed into a marina for leisure boats, and a large hotel was built near the beach to attract visitors. Several new residential buildings and apartments were constructed, particularly in the western area where farmland once existed. The woodland was partly cleared to make space for a shopping centre and a car park.\nTransportation infrastructure was also upgraded considerably. The main road was widened, and a new bridge connected the two sides of the town more efficiently. Furthermore, more public facilities such as restaurants and entertainment areas appeared along the coastline, reflecting the town’s shift towards tourism and modern urban living.', 7, '2026-05-19 20:22:36', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(18, 3, 'The diagram illustrates the process of municipal waste recycling into construction materials. Summarise the process by selecting and reporting the main stages.', 'The two maps illustrate the changes that took place in a small coastal town between 1990 and 2025.\nOverall, the town experienced significant development over the 35-year period. Many new facilities were constructed, including residential areas, tourist attractions, and improved transport infrastructure, while some natural and undeveloped areas disappeared.\nIn 1990, the town was relatively small and simple in structure. A fishing harbour was located on the eastern coast, while a small beach lay to the south. There were only a few houses situated near the main road, and the western side of the town consisted mainly of farmland and woodland. In addition, there was a small local market in the town centre.\nBy 2025, the town had become far more urbanised and tourism-oriented. The fishing harbour had been transformed into a marina for leisure boats, and a large hotel was built near the beach to attract visitors. Several new residential buildings and apartments were constructed, particularly in the western area where farmland once existed. The woodland was partly cleared to make space for a shopping centre and a car park.\nTransportation infrastructure was also upgraded considerably. The main road was widened, and a new bridge connected the two sides of the town more efficiently. Furthermore, more public facilities such as restaurants and entertainment areas appeared along the coastline, reflecting the town’s shift towards tourism and modern urban living.', 7, '2026-05-19 20:29:12', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(19, 3, 'The two maps below show a small coastal town in 1990 and the same town in 2025. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', 'The two maps illustrate the changes that took place in a small coastal town between 1990 and 2025.\nOverall, the town experienced significant development over the 35-year period. Many new facilities were constructed, including residential areas, tourist attractions, and improved transport infrastructure, while some natural and undeveloped areas disappeared.\nIn 1990, the town was relatively small and simple in structure. A fishing harbour was located on the eastern coast, while a small beach lay to the south. There were only a few houses situated near the main road, and the western side of the town consisted mainly of farmland and woodland. In addition, there was a small local market in the town centre.\nBy 2025, the town had become far more urbanised and tourism-oriented. The fishing harbour had been transformed into a marina for leisure boats, and a large hotel was built near the beach to attract visitors. Several new residential buildings and apartments were constructed, particularly in the western area where farmland once existed. The woodland was partly cleared to make space for a shopping centre and a car park.\nTransportation infrastructure was also upgraded considerably. The main road was widened, and a new bridge connected the two sides of the town more efficiently. Furthermore, more public facilities such as restaurants and entertainment areas appeared along the coastline, reflecting the town’s shift towards tourism and modern urban living.', 6, '2026-05-19 20:46:45', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(20, 3, 'The two maps below show a small coastal town in 1990 and the same town in 2025. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', 'The two maps illustrate the changes that took place in a small coastal town between 1990 and 2025.\nOverall, the town experienced significant development over the 35-year period. Many new facilities were constructed, including residential areas, tourist attractions, and improved transport infrastructure, while some natural and undeveloped areas disappeared.\nIn 1990, the town was relatively small and simple in structure. A fishing harbour was located on the eastern coast, while a small beach lay to the south. There were only a few houses situated near the main road, and the western side of the town consisted mainly of farmland and woodland. In addition, there was a small local market in the town centre.\nBy 2025, the town had become far more urbanised and tourism-oriented. The fishing harbour had been transformed into a marina for leisure boats, and a large hotel was built near the beach to attract visitors. Several new residential buildings and apartments were constructed, particularly in the western area where farmland once existed. The woodland was partly cleared to make space for a shopping centre and a car park.\nTransportation infrastructure was also upgraded considerably. The main road was widened, and a new bridge connected the two sides of the town more efficiently. Furthermore, more public facilities such as restaurants and entertainment areas appeared along the coastline, reflecting the town’s shift towards tourism and modern urban living.', 6.5, '2026-05-19 20:49:47', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(21, 3, 'The two maps below show a small coastal town in 1990 and the same town in 2025. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', 'The maps compare the layout of a small seaside town in 1990 with its appearance in 2025.\nOverall, the town changed dramatically during this period, becoming more modern and developed. Several new buildings and facilities were added, while some traditional and natural features were removed or replaced.\nIn 1990, the town was mainly a quiet rural area with limited infrastructure. There was a small fishing port on the coast and only a few residential houses near the centre. Large areas of farmland and trees occupied the western side of the town. A narrow road ran through the town, connecting the market with the harbour and beach area.\nBy 2025, considerable urban development had taken place. The fishing port was converted into a modern marina, and a number of tourist facilities, including cafés and a seaside hotel, were built along the coast. The former farmland was replaced by housing estates and commercial buildings such as shops and restaurants. In addition, the road system was improved significantly, with wider streets and a newly constructed bridge that enhanced transportation across the town.\nAnother noticeable change is that the town became much more crowded and economically focused on tourism rather than fishing and agriculture. Despite these developments, the beach remained in the same location, although it appeared to be more organised for visitors in 2025.', 7, '2026-05-19 20:54:23', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(22, 3, 'The bar chart below shows the percentage of people in three age groups (18-30, 31-50, 51+) who used the internet daily in four countries in 2023. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', 'Overall, younger individuals were more likely to utilize the internet daily than older generations in all four countries. In addition, the percentage of daily internet users gradually declined with age.\nThe 18–30 age group recorded the highest figures in every country, ranging from 85% in Country D to 95% in Country A. Meanwhile, individuals aged 31–50 showed slightly lower proportions, with percentages varying between 70% and 82%.\nBy contrast, the oldest age group (51+) had the lowest levels of internet usage. Country A still had a relatively high figure of 60%, whereas Country D recorded the lowest percentage overall, at only 40%.\nAnother notable feature is that Country A consistently had the highest rates across all age groups, while Country D ranked last in each category. The gap between the youngest and oldest users was also considerable, particularly in Countries C and D.', 6, '2026-05-19 21:07:58', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(23, 3, 'The chart below shows energy usage by sector from 2000 to 2020.', 'Overall, younger individuals were more likely to utilize the internet daily than older generations in all four countries. In addition, the percentage of daily internet users gradually declined with age.\nThe 18–30 age group recorded the highest figures in every country, ranging from 85% in Country D to 95% in Country A. Meanwhile, individuals aged 31–50 showed slightly lower proportions, with percentages varying between 70% and 82%.\nBy contrast, the oldest age group (51+) had the lowest levels of internet usage. Country A still had a relatively high figure of 60%, whereas Country D recorded the lowest percentage overall, at only 40%.\nAnother notable feature is that Country A consistently had the highest rates across all age groups, while Country D ranked last in each category. The gap between the youngest and oldest users was also considerable, particularly in Countries C and D.', 6, '2026-05-19 21:08:39', NULL, 0.0, 0.0, 0.0, 0.0, NULL, NULL, 2),
(24, 3, 'The bar chart compares the proportion of household expenditure on transport, food, and housing in three cities in 2010 and 2020. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', 'Overall, younger individuals were more likely to utilize the internet daily than older generations in all four countries. In addition, the percentage of daily internet users gradually declined with age.\nThe 18–30 age group recorded the highest figures in every country, ranging from 85% in Country D to 95% in Country A. Meanwhile, individuals aged 31–50 showed slightly lower proportions, with percentages varying between 70% and 82%.\nBy contrast, the oldest age group (51+) had the lowest levels of internet usage. Country A still had a relatively high figure of 60%, whereas Country D recorded the lowest percentage overall, at only 40%.\nAnother notable feature is that Country A consistently had the highest rates across all age groups, while Country D ranked last in each category. The gap between the youngest and oldest users was also considerable, particularly in Countries C and D.', 6.5, '2026-05-19 21:50:40', '⚠️ WARNING: Your essay may not fully address the topic.\n\n✓ Task Achievement: Good coverage of the task. Try to develop your main ideas more fully.\n\n✅ Coherence & Cohesion: Well-organized essay with clear progression of ideas.\n\n✅ Lexical Resource: Good vocabulary range with some less common words.\n\n✅ Grammatical Range: Good variety of sentence structures with few errors.\n\n📝 Word Count: 143 words. Minimum required: 150. Develop your ideas further.\n\nStrengths: Clear structure, Relevant response\n\nAreas for Improvement: Align your content to address all aspects of the writing prompt topic., Expand lexical range, Use more complex sentences', 4.5, 7.3, 7.1, 7.2, NULL, 143, 1),
(25, 3, 'The chart below shows energy usage by sector from 2000 to 2020.', 'Overall, younger individuals were more likely to use the internet daily than older generations in all four countries. In addition, the percentage of daily internet users gradually declined with age.\nThe 18–30 age group recorded the highest figures in every country, ranging from 85% in Country D to 95% in Country A. Meanwhile, people aged 31–50 showed slightly lower proportions, with percentages varying between 70% and 82%.\nBy contrast, the oldest age group (51+) had the lowest levels of internet usage. Country A still had a relatively high figure of 60%, whereas Country D recorded the lowest percentage overall, at only 40%.\nAnother notable feature is that Country A consistently had the highest rates across all age groups, while Country D ranked last in each category. The gap between the youngest and oldest users was also considerable, particularly in Countries C and D.', 5.5, '2026-05-19 21:53:50', '✓ Task Achievement: Good coverage of the task. Try to develop your main ideas more fully.\n\n⚠ Coherence & Cohesion: Work on paragraph structure and use more connectors (However, Furthermore, etc.).\n\n⚠ Lexical Resource: Expand your vocabulary. Avoid repetition of words.\n\n⚠ Grammatical Range: Review basic grammar rules. Practice complex sentence structures.\n\n📝 Word Count: 143 words. Minimum required: 150. Develop your ideas further.\n\nStrengths: Clear structure, Relevant response\n\nAreas for Improvement: Expand lexical range, Use more complex sentences', 6.0, 5.4, 5.6, 5.5, NULL, 143, 1),
(26, 3, 'The line graph shows the percentage of commuters using four transport modes between 2005 and 2025 in a major city. Summarise the information by selecting and reporting the main features.', 'Overall, younger people were more likely to use the internet daily than older generations in all four countries. In addition, the percentage of daily internet users gradually declined with age.\nThe 18–30 age group recorded the highest figures in every country, ranging from 85% in Country D to 95% in Country A. Meanwhile, people aged 31–50 showed slightly lower proportions, with percentages varying between 70% and 82%.\nBy contrast, the oldest age group (51+) had the lowest levels of internet usage. Country A still had a relatively high figure of 60%, whereas Country D recorded the lowest percentage overall, at only 40%.\nAnother notable feature is that Country A consistently had the highest rates across all age groups, while Country D ranked last in each category. The gap between the youngest and oldest users was also considerable, particularly in Countries C and D.', 6, '2026-05-19 21:54:27', '✓ Task Achievement: Good coverage of the task. Try to develop your main ideas more fully.\n\n⚠ Coherence & Cohesion: Work on paragraph structure and use more connectors (However, Furthermore, etc.).\n\n⚠ Lexical Resource: Expand your vocabulary. Avoid repetition of words.\n\n✓ Grammatical Range: Generally accurate. Try using more complex sentences.\n\n📝 Word Count: 143 words. Minimum required: 150. Develop your ideas further.\n\nStrengths: Clear structure, Relevant response\n\nAreas for Improvement: Expand lexical range, Use more complex sentences', 6.0, 5.4, 5.9, 6.0, NULL, 143, 1),
(27, 3, 'The bar chart below shows the percentage of people in three age groups (18-30, 31-50, 51+) who used the internet daily in four countries in 2023. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', 'Overall, younger people were more likely to use the internet daily than older generations in all four countries. In addition, the percentage of daily internet users gradually declined with age.\nThe 18–30 age group recorded the highest figures in every country, ranging from 85% in Country D to 95% in Country A. Meanwhile, people aged 31–50 showed slightly lower proportions, with percentages varying between 70% and 82%.\nBy contrast, the oldest age group (51+) had the lowest levels of internet usage. Country A still had a relatively high figure of 60%, whereas Country D recorded the lowest percentage overall, at only 40%.\nAnother notable feature is that Country A consistently had the highest rates across all age groups, while Country D ranked last in each category. The gap between the youngest and oldest users was also considerable, particularly in Countries C and D.', 6, '2026-05-19 21:54:50', '⚠ Task Achievement: Ensure you address all parts of the question. Provide more specific examples.\n\n✓ Coherence & Cohesion: Generally well-organized. Use more varied linking words.\n\n⚠ Lexical Resource: Expand your vocabulary. Avoid repetition of words.\n\n✓ Grammatical Range: Generally accurate. Try using more complex sentences.\n\n📝 Word Count: 143 words. Minimum required: 150. Develop your ideas further.\n\nStrengths: Clear structure, Relevant response\n\nAreas for Improvement: Expand lexical range, Use more complex sentences', 5.7, 6.4, 5.8, 6.4, NULL, 143, 1),
(28, 3, 'The line graph shows the percentage of commuters using four transport modes between 2005 and 2025 in a major city. Summarise the information by selecting and reporting the main features.', 'The maps compare the layout of anegligiblel seaside town in 1990 with its appearance in 2025.\nOverall, the town changed dramatically during this period, becoming more modern and developed. Severalmodernw buildings and facilities were added, while some traditional and natural features were removed or replaced.\nIn 1990, the town was mainly a quiet rural area with limited infrastructure. There was anegligiblel fishing port on the coast and only a few residential houses near the centre. Large areas of farmland and trees occupied the western side of the town. A narrow road ran through the town, connecting the market with the harbour and beach area.\nBy 2025, considerable urban development had taken place. The fishing port was converted into a modern marina, and a number of tourist facilities, including cafés and a seaside hotel, were built along the coast. The former farmland was replaced by housing estates and commercial buildings such as shops and restaurants. In addition, the road system was improved significantly, with wider streets and a newly constructed bridge that enhanced transportation across the town.\nAnother noticeablemodifye is that the town became much more crowded and economically focused on tourism rather than fishing and agriculture. Despite these developments, the beach remained in the same location, although it appeared to be more organised for visitors in 2025.', 7, '2026-05-20 08:31:01', '✅ Task Achievement: Excellent! You addressed all parts of the task with relevant, extended ideas.\n\n✓ Coherence & Cohesion: Generally well-organized. Use more varied linking words.\n\n✓ Lexical Resource: Adequate vocabulary. Try to use more topic-specific vocabulary.\n\n✅ Grammatical Range: Good variety of sentence structures with few errors.\n\n📝 Word Count: 217 words. Meets the requirement.\n\nStrengths: Clear structure, Relevant response\n\nAreas for Improvement: Expand lexical range, Use more complex sentences', 7.4, 6.9, 6.7, 7.6, NULL, 217, 1);

-- --------------------------------------------------------

--
-- Table structure for table `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `tb_admin_actions`
--
ALTER TABLE `tb_admin_actions`
  ADD PRIMARY KEY (`action_id`),
  ADD KEY `idx_admin_action_user` (`admin_id`);

--
-- Indexes for table `tb_ai_roadmaps`
--
ALTER TABLE `tb_ai_roadmaps`
  ADD PRIMARY KEY (`roadmap_id`),
  ADD KEY `idx_roadmap_user` (`user_id`);

--
-- Indexes for table `tb_ai_roadmap_steps`
--
ALTER TABLE `tb_ai_roadmap_steps`
  ADD PRIMARY KEY (`step_id`),
  ADD KEY `idx_step_roadmap` (`roadmap_id`),
  ADD KEY `idx_step_lesson` (`lesson_id`);

--
-- Indexes for table `tb_ai_skill_analysis`
--
ALTER TABLE `tb_ai_skill_analysis`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idx_skill_user` (`user_id`);

--
-- Indexes for table `tb_announcements`
--
ALTER TABLE `tb_announcements`
  ADD PRIMARY KEY (`announcement_id`),
  ADD KEY `fk_ann_teacher` (`teacher_id`);

--
-- Indexes for table `tb_answers`
--
ALTER TABLE `tb_answers`
  ADD PRIMARY KEY (`answer_id`);

--
-- Indexes for table `tb_audit_logs`
--
ALTER TABLE `tb_audit_logs`
  ADD PRIMARY KEY (`log_id`),
  ADD KEY `idx_audit_user` (`user_id`),
  ADD KEY `idx_audit_admin` (`admin_id`),
  ADD KEY `idx_audit_entity` (`entity_type`,`entity_id`),
  ADD KEY `idx_audit_created` (`created_at`);

--
-- Indexes for table `tb_conversations`
--
ALTER TABLE `tb_conversations`
  ADD PRIMARY KEY (`conversation_id`),
  ADD KEY `fk_conv_student` (`student_id`),
  ADD KEY `fk_conv_teacher` (`teacher_id`);

--
-- Indexes for table `tb_courses`
--
ALTER TABLE `tb_courses`
  ADD PRIMARY KEY (`course_id`),
  ADD UNIQUE KEY `uk_courses_slug` (`slug`),
  ADD KEY `idx_course_skill` (`skill_type`),
  ADD KEY `idx_course_published` (`is_published`);

--
-- Indexes for table `tb_gamification`
--
ALTER TABLE `tb_gamification`
  ADD PRIMARY KEY (`achievement_id`),
  ADD UNIQUE KEY `uk_gamification_code` (`achievement_code`),
  ADD KEY `idx_achievement_active` (`is_active`);

--
-- Indexes for table `tb_grade_disputes`
--
ALTER TABLE `tb_grade_disputes`
  ADD PRIMARY KEY (`dispute_id`),
  ADD KEY `fk_dispute_user` (`user_id`),
  ADD KEY `fk_dispute_teacher` (`reviewed_by`);

--
-- Indexes for table `tb_lessons`
--
ALTER TABLE `tb_lessons`
  ADD PRIMARY KEY (`lesson_id`),
  ADD KEY `idx_lesson_module` (`module_id`),
  ADD KEY `idx_lesson_skill` (`skill_type`);

--
-- Indexes for table `tb_lesson_contents`
--
ALTER TABLE `tb_lesson_contents`
  ADD PRIMARY KEY (`content_id`),
  ADD KEY `idx_content_lesson` (`lesson_id`);

--
-- Indexes for table `tb_listening_materials`
--
ALTER TABLE `tb_listening_materials`
  ADD PRIMARY KEY (`material_id`),
  ADD KEY `idx_listening_lesson` (`lesson_id`),
  ADD KEY `idx_listening_difficulty` (`difficulty_level`);

--
-- Indexes for table `tb_listening_questions`
--
ALTER TABLE `tb_listening_questions`
  ADD PRIMARY KEY (`question_id`),
  ADD KEY `idx_material_id` (`material_id`),
  ADD KEY `idx_difficulty` (`question_type`),
  ADD KEY `idx_listening_question_material` (`material_id`);

--
-- Indexes for table `tb_messages`
--
ALTER TABLE `tb_messages`
  ADD PRIMARY KEY (`message_id`),
  ADD KEY `fk_msg_conv` (`conversation_id`),
  ADD KEY `fk_msg_sender` (`sender_id`);

--
-- Indexes for table `tb_modules`
--
ALTER TABLE `tb_modules`
  ADD PRIMARY KEY (`module_id`),
  ADD KEY `idx_module_course` (`course_id`);

--
-- Indexes for table `tb_notifications`
--
ALTER TABLE `tb_notifications`
  ADD PRIMARY KEY (`notification_id`),
  ADD KEY `idx_notif_user` (`user_id`),
  ADD KEY `idx_notif_read` (`is_read`),
  ADD KEY `idx_notif_created` (`created_at`);

--
-- Indexes for table `tb_questions`
--
ALTER TABLE `tb_questions`
  ADD PRIMARY KEY (`question_id`),
  ADD KEY `idx_question_section` (`section_id`);

--
-- Indexes for table `tb_reading_passages`
--
ALTER TABLE `tb_reading_passages`
  ADD PRIMARY KEY (`passage_id`),
  ADD KEY `idx_difficulty` (`difficulty_level`),
  ADD KEY `idx_topic` (`topic_category`),
  ADD KEY `idx_reading_lesson` (`lesson_id`),
  ADD KEY `idx_reading_difficulty` (`difficulty_level`);

--
-- Indexes for table `tb_reading_questions`
--
ALTER TABLE `tb_reading_questions`
  ADD PRIMARY KEY (`question_id`),
  ADD KEY `idx_passage_id` (`passage_id`),
  ADD KEY `idx_type` (`question_type`),
  ADD KEY `idx_reading_question_passage` (`passage_id`);

--
-- Indexes for table `tb_reports`
--
ALTER TABLE `tb_reports`
  ADD PRIMARY KEY (`report_id`),
  ADD KEY `idx_report_status` (`status`),
  ADD KEY `idx_report_type` (`report_type`),
  ADD KEY `idx_report_user` (`user_id`);

--
-- Indexes for table `tb_roadmap_suggestions`
--
ALTER TABLE `tb_roadmap_suggestions`
  ADD PRIMARY KEY (`suggestion_id`),
  ADD KEY `fk_rsugg_teacher` (`teacher_id`),
  ADD KEY `fk_rsugg_student` (`student_id`),
  ADD KEY `fk_rsugg_roadmap` (`roadmap_id`);

--
-- Indexes for table `tb_speaking_sessions`
--
ALTER TABLE `tb_speaking_sessions`
  ADD PRIMARY KEY (`session_id`),
  ADD KEY `idx_speaking_user` (`user_id`),
  ADD KEY `idx_speaking_created` (`created_at`);

--
-- Indexes for table `tb_speaking_topics`
--
ALTER TABLE `tb_speaking_topics`
  ADD PRIMARY KEY (`topic_id`),
  ADD KEY `idx_topic_part` (`part`),
  ADD KEY `idx_topic_difficulty` (`difficulty_level`),
  ADD KEY `idx_topic_active` (`is_active`),
  ADD KEY `fk_speaking_created_by` (`created_by`),
  ADD KEY `fk_speaking_reviewed_by` (`reviewed_by`);

--
-- Indexes for table `tb_speaking_topic_parts`
--
ALTER TABLE `tb_speaking_topic_parts`
  ADD PRIMARY KEY (`part_id`),
  ADD KEY `idx_part_topic` (`topic_id`);

--
-- Indexes for table `tb_system_settings`
--
ALTER TABLE `tb_system_settings`
  ADD PRIMARY KEY (`setting_key`);

--
-- Indexes for table `tb_teacher_assignments`
--
ALTER TABLE `tb_teacher_assignments`
  ADD PRIMARY KEY (`assignment_id`),
  ADD KEY `fk_asgn_test` (`test_id`),
  ADD KEY `fk_asgn_teacher` (`teacher_id`),
  ADD KEY `fk_asgn_student` (`student_id`);

--
-- Indexes for table `tb_teacher_certificates`
--
ALTER TABLE `tb_teacher_certificates`
  ADD PRIMARY KEY (`certificate_id`),
  ADD KEY `FK_Teacher_Certificates` (`teacher_id`);

--
-- Indexes for table `tb_teacher_profiles`
--
ALTER TABLE `tb_teacher_profiles`
  ADD PRIMARY KEY (`teacher_id`);

--
-- Indexes for table `tb_tests`
--
ALTER TABLE `tb_tests`
  ADD PRIMARY KEY (`test_id`);

--
-- Indexes for table `tb_test_sections`
--
ALTER TABLE `tb_test_sections`
  ADD PRIMARY KEY (`section_id`),
  ADD KEY `idx_section_test` (`test_id`);

--
-- Indexes for table `tb_users`
--
ALTER TABLE `tb_users`
  ADD PRIMARY KEY (`user_id`),
  ADD UNIQUE KEY `uk_users_email` (`email`),
  ADD UNIQUE KEY `uk_users_username` (`username`),
  ADD KEY `idx_users_deleted` (`is_deleted`),
  ADD KEY `idx_users_verified` (`email_verified`);

--
-- Indexes for table `tb_user_achievements`
--
ALTER TABLE `tb_user_achievements`
  ADD PRIMARY KEY (`user_achievement_id`),
  ADD UNIQUE KEY `uk_user_achievement` (`user_id`,`achievement_id`),
  ADD KEY `idx_user_ach_user` (`user_id`),
  ADD KEY `fk_user_ach_achievement` (`achievement_id`);

--
-- Indexes for table `tb_user_answers`
--
ALTER TABLE `tb_user_answers`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idx_user_answer_attempt` (`attempt_id`),
  ADD KEY `idx_user_answer_question` (`question_id`);

--
-- Indexes for table `tb_user_daily_activity`
--
ALTER TABLE `tb_user_daily_activity`
  ADD PRIMARY KEY (`activity_id`),
  ADD UNIQUE KEY `uk_user_date` (`user_id`,`activity_date`),
  ADD KEY `idx_activity_user` (`user_id`),
  ADD KEY `idx_activity_date` (`activity_date`);

--
-- Indexes for table `tb_user_goals`
--
ALTER TABLE `tb_user_goals`
  ADD PRIMARY KEY (`goal_id`),
  ADD KEY `idx_user_goals_user` (`user_id`);

--
-- Indexes for table `tb_user_learning_progress`
--
ALTER TABLE `tb_user_learning_progress`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idx_progress_user` (`user_id`),
  ADD KEY `idx_progress_lesson` (`lesson_id`),
  ADD KEY `idx_progress_user_lesson` (`user_id`,`lesson_id`);

--
-- Indexes for table `tb_user_placement_results`
--
ALTER TABLE `tb_user_placement_results`
  ADD PRIMARY KEY (`result_id`),
  ADD KEY `idx_placement_user` (`user_id`),
  ADD KEY `idx_placement_date` (`completed_at`),
  ADD KEY `fk_placement_test` (`test_id`);

--
-- Indexes for table `tb_user_practice_attempts`
--
ALTER TABLE `tb_user_practice_attempts`
  ADD PRIMARY KEY (`attempt_id`),
  ADD KEY `idx_user_id` (`user_id`),
  ADD KEY `idx_skill_type` (`skill_type`),
  ADD KEY `idx_attempt_date` (`attempt_date`),
  ADD KEY `idx_practice_user` (`user_id`),
  ADD KEY `idx_practice_skill` (`skill_type`),
  ADD KEY `idx_practice_date` (`attempt_date`),
  ADD KEY `fk_practice_listening_question` (`listening_question_id`),
  ADD KEY `fk_practice_reading_question` (`reading_question_id`);

--
-- Indexes for table `tb_user_profiles`
--
ALTER TABLE `tb_user_profiles`
  ADD PRIMARY KEY (`user_id`);

--
-- Indexes for table `tb_user_sessions`
--
ALTER TABLE `tb_user_sessions`
  ADD PRIMARY KEY (`session_id`),
  ADD UNIQUE KEY `uk_user_sessions_refresh_token` (`refresh_token`),
  ADD KEY `idx_user_sessions_user` (`user_id`),
  ADD KEY `idx_user_sessions_token` (`refresh_token`),
  ADD KEY `idx_user_sessions_expires` (`expires_at`);

--
-- Indexes for table `tb_user_test_attempts`
--
ALTER TABLE `tb_user_test_attempts`
  ADD PRIMARY KEY (`attempt_id`),
  ADD KEY `idx_test_user` (`user_id`),
  ADD KEY `idx_test_test` (`test_id`),
  ADD KEY `idx_test_created` (`created_at`);

--
-- Indexes for table `tb_vocabulary`
--
ALTER TABLE `tb_vocabulary`
  ADD PRIMARY KEY (`vocab_id`),
  ADD KEY `idx_vocab_band` (`difficulty`),
  ADD KEY `idx_vocab_category` (`ielts_topic`),
  ADD KEY `idx_vocab_frequency` (`usage_frequency`);

--
-- Indexes for table `tb_writing_prompts`
--
ALTER TABLE `tb_writing_prompts`
  ADD PRIMARY KEY (`prompt_id`),
  ADD KEY `idx_prompt_task` (`task_type`),
  ADD KEY `idx_prompt_difficulty` (`difficulty_level`),
  ADD KEY `idx_prompt_active` (`is_active`),
  ADD KEY `fk_writing_created_by` (`created_by`),
  ADD KEY `fk_writing_reviewed_by` (`reviewed_by`);

--
-- Indexes for table `tb_writing_submissions`
--
ALTER TABLE `tb_writing_submissions`
  ADD PRIMARY KEY (`submission_id`),
  ADD KEY `idx_writing_user` (`user_id`),
  ADD KEY `idx_writing_created` (`created_at`);

--
-- Indexes for table `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `tb_admin_actions`
--
ALTER TABLE `tb_admin_actions`
  MODIFY `action_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `tb_ai_roadmaps`
--
ALTER TABLE `tb_ai_roadmaps`
  MODIFY `roadmap_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=38;

--
-- AUTO_INCREMENT for table `tb_ai_roadmap_steps`
--
ALTER TABLE `tb_ai_roadmap_steps`
  MODIFY `step_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=51;

--
-- AUTO_INCREMENT for table `tb_ai_skill_analysis`
--
ALTER TABLE `tb_ai_skill_analysis`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `tb_announcements`
--
ALTER TABLE `tb_announcements`
  MODIFY `announcement_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `tb_answers`
--
ALTER TABLE `tb_answers`
  MODIFY `answer_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `tb_audit_logs`
--
ALTER TABLE `tb_audit_logs`
  MODIFY `log_id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=50;

--
-- AUTO_INCREMENT for table `tb_conversations`
--
ALTER TABLE `tb_conversations`
  MODIFY `conversation_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `tb_courses`
--
ALTER TABLE `tb_courses`
  MODIFY `course_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2003;

--
-- AUTO_INCREMENT for table `tb_gamification`
--
ALTER TABLE `tb_gamification`
  MODIFY `achievement_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `tb_grade_disputes`
--
ALTER TABLE `tb_grade_disputes`
  MODIFY `dispute_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `tb_lessons`
--
ALTER TABLE `tb_lessons`
  MODIFY `lesson_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2013;

--
-- AUTO_INCREMENT for table `tb_lesson_contents`
--
ALTER TABLE `tb_lesson_contents`
  MODIFY `content_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2013;

--
-- AUTO_INCREMENT for table `tb_listening_materials`
--
ALTER TABLE `tb_listening_materials`
  MODIFY `material_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2005;

--
-- AUTO_INCREMENT for table `tb_listening_questions`
--
ALTER TABLE `tb_listening_questions`
  MODIFY `question_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2021;

--
-- AUTO_INCREMENT for table `tb_messages`
--
ALTER TABLE `tb_messages`
  MODIFY `message_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `tb_modules`
--
ALTER TABLE `tb_modules`
  MODIFY `module_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2005;

--
-- AUTO_INCREMENT for table `tb_notifications`
--
ALTER TABLE `tb_notifications`
  MODIFY `notification_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `tb_questions`
--
ALTER TABLE `tb_questions`
  MODIFY `question_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8089;

--
-- AUTO_INCREMENT for table `tb_reading_passages`
--
ALTER TABLE `tb_reading_passages`
  MODIFY `passage_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2004;

--
-- AUTO_INCREMENT for table `tb_reading_questions`
--
ALTER TABLE `tb_reading_questions`
  MODIFY `question_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2016;

--
-- AUTO_INCREMENT for table `tb_reports`
--
ALTER TABLE `tb_reports`
  MODIFY `report_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `tb_roadmap_suggestions`
--
ALTER TABLE `tb_roadmap_suggestions`
  MODIFY `suggestion_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `tb_speaking_sessions`
--
ALTER TABLE `tb_speaking_sessions`
  MODIFY `session_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT for table `tb_speaking_topics`
--
ALTER TABLE `tb_speaking_topics`
  MODIFY `topic_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2005;

--
-- AUTO_INCREMENT for table `tb_speaking_topic_parts`
--
ALTER TABLE `tb_speaking_topic_parts`
  MODIFY `part_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2009;

--
-- AUTO_INCREMENT for table `tb_teacher_assignments`
--
ALTER TABLE `tb_teacher_assignments`
  MODIFY `assignment_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `tb_teacher_certificates`
--
ALTER TABLE `tb_teacher_certificates`
  MODIFY `certificate_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `tb_tests`
--
ALTER TABLE `tb_tests`
  MODIFY `test_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2009;

--
-- AUTO_INCREMENT for table `tb_test_sections`
--
ALTER TABLE `tb_test_sections`
  MODIFY `section_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2012;

--
-- AUTO_INCREMENT for table `tb_users`
--
ALTER TABLE `tb_users`
  MODIFY `user_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT for table `tb_user_achievements`
--
ALTER TABLE `tb_user_achievements`
  MODIFY `user_achievement_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `tb_user_answers`
--
ALTER TABLE `tb_user_answers`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=46;

--
-- AUTO_INCREMENT for table `tb_user_daily_activity`
--
ALTER TABLE `tb_user_daily_activity`
  MODIFY `activity_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `tb_user_goals`
--
ALTER TABLE `tb_user_goals`
  MODIFY `goal_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `tb_user_learning_progress`
--
ALTER TABLE `tb_user_learning_progress`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `tb_user_placement_results`
--
ALTER TABLE `tb_user_placement_results`
  MODIFY `result_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `tb_user_practice_attempts`
--
ALTER TABLE `tb_user_practice_attempts`
  MODIFY `attempt_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=39;

--
-- AUTO_INCREMENT for table `tb_user_sessions`
--
ALTER TABLE `tb_user_sessions`
  MODIFY `session_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;

--
-- AUTO_INCREMENT for table `tb_user_test_attempts`
--
ALTER TABLE `tb_user_test_attempts`
  MODIFY `attempt_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `tb_vocabulary`
--
ALTER TABLE `tb_vocabulary`
  MODIFY `vocab_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=43;

--
-- AUTO_INCREMENT for table `tb_writing_prompts`
--
ALTER TABLE `tb_writing_prompts`
  MODIFY `prompt_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2006;

--
-- AUTO_INCREMENT for table `tb_writing_submissions`
--
ALTER TABLE `tb_writing_submissions`
  MODIFY `submission_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=29;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `tb_admin_actions`
--
ALTER TABLE `tb_admin_actions`
  ADD CONSTRAINT `fk_admin_action_user` FOREIGN KEY (`admin_id`) REFERENCES `tb_users` (`user_id`) ON DELETE SET NULL;

--
-- Constraints for table `tb_ai_roadmaps`
--
ALTER TABLE `tb_ai_roadmaps`
  ADD CONSTRAINT `fk_roadmap_user` FOREIGN KEY (`user_id`) REFERENCES `tb_users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_ai_roadmap_steps`
--
ALTER TABLE `tb_ai_roadmap_steps`
  ADD CONSTRAINT `fk_step_lesson` FOREIGN KEY (`lesson_id`) REFERENCES `tb_lessons` (`lesson_id`) ON DELETE SET NULL,
  ADD CONSTRAINT `fk_step_roadmap` FOREIGN KEY (`roadmap_id`) REFERENCES `tb_ai_roadmaps` (`roadmap_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_ai_skill_analysis`
--
ALTER TABLE `tb_ai_skill_analysis`
  ADD CONSTRAINT `fk_analysis_user` FOREIGN KEY (`user_id`) REFERENCES `tb_users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_announcements`
--
ALTER TABLE `tb_announcements`
  ADD CONSTRAINT `fk_ann_teacher` FOREIGN KEY (`teacher_id`) REFERENCES `tb_users` (`user_id`);

--
-- Constraints for table `tb_audit_logs`
--
ALTER TABLE `tb_audit_logs`
  ADD CONSTRAINT `fk_audit_admin` FOREIGN KEY (`admin_id`) REFERENCES `tb_users` (`user_id`) ON DELETE SET NULL,
  ADD CONSTRAINT `fk_audit_user` FOREIGN KEY (`user_id`) REFERENCES `tb_users` (`user_id`) ON DELETE SET NULL;

--
-- Constraints for table `tb_conversations`
--
ALTER TABLE `tb_conversations`
  ADD CONSTRAINT `fk_conv_student` FOREIGN KEY (`student_id`) REFERENCES `tb_users` (`user_id`),
  ADD CONSTRAINT `fk_conv_teacher` FOREIGN KEY (`teacher_id`) REFERENCES `tb_users` (`user_id`);

--
-- Constraints for table `tb_grade_disputes`
--
ALTER TABLE `tb_grade_disputes`
  ADD CONSTRAINT `fk_dispute_teacher` FOREIGN KEY (`reviewed_by`) REFERENCES `tb_users` (`user_id`),
  ADD CONSTRAINT `fk_dispute_user` FOREIGN KEY (`user_id`) REFERENCES `tb_users` (`user_id`);

--
-- Constraints for table `tb_lessons`
--
ALTER TABLE `tb_lessons`
  ADD CONSTRAINT `fk_lesson_module` FOREIGN KEY (`module_id`) REFERENCES `tb_modules` (`module_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_lesson_contents`
--
ALTER TABLE `tb_lesson_contents`
  ADD CONSTRAINT `fk_content_lesson` FOREIGN KEY (`lesson_id`) REFERENCES `tb_lessons` (`lesson_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_listening_materials`
--
ALTER TABLE `tb_listening_materials`
  ADD CONSTRAINT `fk_listening_lesson` FOREIGN KEY (`lesson_id`) REFERENCES `tb_lessons` (`lesson_id`) ON DELETE SET NULL;

--
-- Constraints for table `tb_listening_questions`
--
ALTER TABLE `tb_listening_questions`
  ADD CONSTRAINT `fk_listening_question_material` FOREIGN KEY (`material_id`) REFERENCES `tb_listening_materials` (`material_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_messages`
--
ALTER TABLE `tb_messages`
  ADD CONSTRAINT `fk_msg_conv` FOREIGN KEY (`conversation_id`) REFERENCES `tb_conversations` (`conversation_id`),
  ADD CONSTRAINT `fk_msg_sender` FOREIGN KEY (`sender_id`) REFERENCES `tb_users` (`user_id`);

--
-- Constraints for table `tb_modules`
--
ALTER TABLE `tb_modules`
  ADD CONSTRAINT `fk_module_course` FOREIGN KEY (`course_id`) REFERENCES `tb_courses` (`course_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_notifications`
--
ALTER TABLE `tb_notifications`
  ADD CONSTRAINT `fk_notif_user` FOREIGN KEY (`user_id`) REFERENCES `tb_users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_questions`
--
ALTER TABLE `tb_questions`
  ADD CONSTRAINT `fk_question_section` FOREIGN KEY (`section_id`) REFERENCES `tb_test_sections` (`section_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_reading_passages`
--
ALTER TABLE `tb_reading_passages`
  ADD CONSTRAINT `fk_reading_lesson` FOREIGN KEY (`lesson_id`) REFERENCES `tb_lessons` (`lesson_id`) ON DELETE SET NULL;

--
-- Constraints for table `tb_reading_questions`
--
ALTER TABLE `tb_reading_questions`
  ADD CONSTRAINT `fk_reading_question_passage` FOREIGN KEY (`passage_id`) REFERENCES `tb_reading_passages` (`passage_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_reports`
--
ALTER TABLE `tb_reports`
  ADD CONSTRAINT `fk_report_user` FOREIGN KEY (`user_id`) REFERENCES `tb_users` (`user_id`) ON DELETE SET NULL;

--
-- Constraints for table `tb_roadmap_suggestions`
--
ALTER TABLE `tb_roadmap_suggestions`
  ADD CONSTRAINT `fk_rsugg_roadmap` FOREIGN KEY (`roadmap_id`) REFERENCES `tb_ai_roadmaps` (`roadmap_id`),
  ADD CONSTRAINT `fk_rsugg_student` FOREIGN KEY (`student_id`) REFERENCES `tb_users` (`user_id`),
  ADD CONSTRAINT `fk_rsugg_teacher` FOREIGN KEY (`teacher_id`) REFERENCES `tb_users` (`user_id`);

--
-- Constraints for table `tb_speaking_sessions`
--
ALTER TABLE `tb_speaking_sessions`
  ADD CONSTRAINT `fk_speaking_user` FOREIGN KEY (`user_id`) REFERENCES `tb_users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_speaking_topics`
--
ALTER TABLE `tb_speaking_topics`
  ADD CONSTRAINT `fk_speaking_created_by` FOREIGN KEY (`created_by`) REFERENCES `tb_users` (`user_id`),
  ADD CONSTRAINT `fk_speaking_reviewed_by` FOREIGN KEY (`reviewed_by`) REFERENCES `tb_users` (`user_id`);

--
-- Constraints for table `tb_speaking_topic_parts`
--
ALTER TABLE `tb_speaking_topic_parts`
  ADD CONSTRAINT `fk_part_topic` FOREIGN KEY (`topic_id`) REFERENCES `tb_speaking_topics` (`topic_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_teacher_assignments`
--
ALTER TABLE `tb_teacher_assignments`
  ADD CONSTRAINT `fk_asgn_student` FOREIGN KEY (`student_id`) REFERENCES `tb_users` (`user_id`),
  ADD CONSTRAINT `fk_asgn_teacher` FOREIGN KEY (`teacher_id`) REFERENCES `tb_users` (`user_id`),
  ADD CONSTRAINT `fk_asgn_test` FOREIGN KEY (`test_id`) REFERENCES `tb_tests` (`test_id`);

--
-- Constraints for table `tb_teacher_certificates`
--
ALTER TABLE `tb_teacher_certificates`
  ADD CONSTRAINT `FK_Teacher_Certificates` FOREIGN KEY (`teacher_id`) REFERENCES `tb_teacher_profiles` (`teacher_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_teacher_profiles`
--
ALTER TABLE `tb_teacher_profiles`
  ADD CONSTRAINT `fk_tprofile_user` FOREIGN KEY (`teacher_id`) REFERENCES `tb_users` (`user_id`);

--
-- Constraints for table `tb_test_sections`
--
ALTER TABLE `tb_test_sections`
  ADD CONSTRAINT `fk_section_test` FOREIGN KEY (`test_id`) REFERENCES `tb_tests` (`test_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_user_achievements`
--
ALTER TABLE `tb_user_achievements`
  ADD CONSTRAINT `fk_user_ach_achievement` FOREIGN KEY (`achievement_id`) REFERENCES `tb_gamification` (`achievement_id`) ON DELETE CASCADE,
  ADD CONSTRAINT `fk_user_ach_user` FOREIGN KEY (`user_id`) REFERENCES `tb_users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_user_answers`
--
ALTER TABLE `tb_user_answers`
  ADD CONSTRAINT `fk_answer_attempt` FOREIGN KEY (`attempt_id`) REFERENCES `tb_user_test_attempts` (`attempt_id`) ON DELETE CASCADE,
  ADD CONSTRAINT `fk_answer_question` FOREIGN KEY (`question_id`) REFERENCES `tb_questions` (`question_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_user_daily_activity`
--
ALTER TABLE `tb_user_daily_activity`
  ADD CONSTRAINT `fk_activity_user` FOREIGN KEY (`user_id`) REFERENCES `tb_users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_user_goals`
--
ALTER TABLE `tb_user_goals`
  ADD CONSTRAINT `fk_goal_user` FOREIGN KEY (`user_id`) REFERENCES `tb_users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_user_learning_progress`
--
ALTER TABLE `tb_user_learning_progress`
  ADD CONSTRAINT `fk_progress_lesson` FOREIGN KEY (`lesson_id`) REFERENCES `tb_lessons` (`lesson_id`) ON DELETE CASCADE,
  ADD CONSTRAINT `fk_progress_user` FOREIGN KEY (`user_id`) REFERENCES `tb_users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_user_placement_results`
--
ALTER TABLE `tb_user_placement_results`
  ADD CONSTRAINT `fk_placement_test` FOREIGN KEY (`test_id`) REFERENCES `tb_tests` (`test_id`) ON DELETE SET NULL,
  ADD CONSTRAINT `fk_placement_user` FOREIGN KEY (`user_id`) REFERENCES `tb_users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_user_practice_attempts`
--
ALTER TABLE `tb_user_practice_attempts`
  ADD CONSTRAINT `fk_practice_listening_question` FOREIGN KEY (`listening_question_id`) REFERENCES `tb_listening_questions` (`question_id`) ON DELETE CASCADE,
  ADD CONSTRAINT `fk_practice_reading_question` FOREIGN KEY (`reading_question_id`) REFERENCES `tb_reading_questions` (`question_id`) ON DELETE CASCADE,
  ADD CONSTRAINT `fk_practice_user` FOREIGN KEY (`user_id`) REFERENCES `tb_users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_user_profiles`
--
ALTER TABLE `tb_user_profiles`
  ADD CONSTRAINT `fk_profile_user` FOREIGN KEY (`user_id`) REFERENCES `tb_users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_user_sessions`
--
ALTER TABLE `tb_user_sessions`
  ADD CONSTRAINT `fk_session_user` FOREIGN KEY (`user_id`) REFERENCES `tb_users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_user_test_attempts`
--
ALTER TABLE `tb_user_test_attempts`
  ADD CONSTRAINT `fk_attempt_test` FOREIGN KEY (`test_id`) REFERENCES `tb_tests` (`test_id`) ON DELETE CASCADE,
  ADD CONSTRAINT `fk_attempt_user` FOREIGN KEY (`user_id`) REFERENCES `tb_users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints for table `tb_writing_prompts`
--
ALTER TABLE `tb_writing_prompts`
  ADD CONSTRAINT `fk_writing_created_by` FOREIGN KEY (`created_by`) REFERENCES `tb_users` (`user_id`),
  ADD CONSTRAINT `fk_writing_reviewed_by` FOREIGN KEY (`reviewed_by`) REFERENCES `tb_users` (`user_id`);

--
-- Constraints for table `tb_writing_submissions`
--
ALTER TABLE `tb_writing_submissions`
  ADD CONSTRAINT `fk_writing_user` FOREIGN KEY (`user_id`) REFERENCES `tb_users` (`user_id`) ON DELETE CASCADE;

-- ========================================================
-- SUPPLEMENTARY DATA: Cambridge IELTS 20 Tests 2, 3, 4
-- Plus additional vocabulary and speaking topics
-- Generated for WebIeltsFree Platform
-- ========================================================

--
-- Additional Courses: Cambridge IELTS 20 Tests 2-4
--
INSERT INTO `tb_courses` (`course_id`, `title`, `target_band`, `skill_type`, `difficulty_level`, `thumbnail_url`, `estimated_hours`, `is_published`, `order_index`, `slug`, `description`) VALUES
(3001, 'Cambridge IELTS 20 - Academic Test 2', 7, 'mixed', 3, '/images/cam20.jpg', 3, 1, 31, 'cambridge-ielts-20-test-2', 'Full academic practice test 2 from Cambridge IELTS 20 with Listening, Reading, Writing and Speaking.'),
(4001, 'Cambridge IELTS 20 - Academic Test 3', 7, 'mixed', 3, '/images/cam20.jpg', 3, 1, 32, 'cambridge-ielts-20-test-3', 'Full academic practice test 3 from Cambridge IELTS 20 with Listening, Reading, Writing and Speaking.'),
(5001, 'Cambridge IELTS 20 - Academic Test 4', 7.5, 'mixed', 4, '/images/cam20.jpg', 3, 1, 33, 'cambridge-ielts-20-test-4', 'Full academic practice test 4 from Cambridge IELTS 20 with Listening, Reading, Writing and Speaking.');


--
-- Modules for Cambridge 20 Tests 2-4
--
INSERT INTO `tb_modules` (`module_id`, `course_id`, `title`, `order_index`, `is_deleted`, `deleted_at`, `updated_at`) VALUES
(3001, 3001, 'Listening - Test 2', 1, 0, NULL, '2026-05-26 12:00:00'),
(3002, 3001, 'Reading - Test 2', 2, 0, NULL, '2026-05-26 12:00:00'),
(3003, 3001, 'Writing - Test 2', 3, 0, NULL, '2026-05-26 12:00:00'),
(3004, 3001, 'Speaking - Test 2', 4, 0, NULL, '2026-05-26 12:00:00'),
(4001, 4001, 'Listening - Test 3', 1, 0, NULL, '2026-05-26 12:00:00'),
(4002, 4001, 'Reading - Test 3', 2, 0, NULL, '2026-05-26 12:00:00'),
(4003, 4001, 'Writing - Test 3', 3, 0, NULL, '2026-05-26 12:00:00'),
(4004, 4001, 'Speaking - Test 3', 4, 0, NULL, '2026-05-26 12:00:00'),
(5001, 5001, 'Listening - Test 4', 1, 0, NULL, '2026-05-26 12:00:00'),
(5002, 5001, 'Reading - Test 4', 2, 0, NULL, '2026-05-26 12:00:00'),
(5003, 5001, 'Writing - Test 4', 3, 0, NULL, '2026-05-26 12:00:00'),
(5004, 5001, 'Speaking - Test 4', 4, 0, NULL, '2026-05-26 12:00:00');


--
-- Lessons for Cambridge 20 Tests 2-4
--
INSERT INTO `tb_lessons` (`lesson_id`, `module_id`, `title`, `skill_type`, `difficulty_level`, `estimated_minutes`, `is_deleted`, `deleted_at`, `updated_at`) VALUES
-- Test 2 Lessons
(3001, 3001, 'Listening Section 1: Hotel Booking Enquiry', 'listening', 2, 8, 0, NULL, '2026-05-26 12:00:00'),
(3002, 3001, 'Listening Section 2: Local Library Services', 'listening', 2, 8, 0, NULL, '2026-05-26 12:00:00'),
(3003, 3001, 'Listening Section 3: Group Assignment Planning', 'listening', 3, 10, 0, NULL, '2026-05-26 12:00:00'),
(3004, 3001, 'Listening Section 4: Renewable Energy Lecture', 'listening', 4, 10, 0, NULL, '2026-05-26 12:00:00'),
(3005, 3002, 'Reading Passage 1: The Rise of Urban Farming', 'reading', 2, 20, 0, NULL, '2026-05-26 12:00:00'),
(3006, 3002, 'Reading Passage 2: Digital Literacy in Education', 'reading', 3, 20, 0, NULL, '2026-05-26 12:00:00'),
(3007, 3002, 'Reading Passage 3: Behavioural Economics and Policy', 'reading', 4, 20, 0, NULL, '2026-05-26 12:00:00'),
(3008, 3003, 'Writing Task 1: Water Consumption Chart', 'writing', 3, 20, 0, NULL, '2026-05-26 12:00:00'),
(3009, 3003, 'Writing Task 2: International Tourism Impact', 'writing', 3, 40, 0, NULL, '2026-05-26 12:00:00'),
(3010, 3004, 'Speaking Part 1: Neighbourhood and Living', 'speaking', 2, 5, 0, NULL, '2026-05-26 12:00:00'),
(3011, 3004, 'Speaking Part 2: Describe a Book', 'speaking', 3, 4, 0, NULL, '2026-05-26 12:00:00'),
(3012, 3004, 'Speaking Part 3: Reading Habits', 'speaking', 4, 5, 0, NULL, '2026-05-26 12:00:00'),
-- Test 3 Lessons
(4001, 4001, 'Listening Section 1: Sports Centre Registration', 'listening', 2, 8, 0, NULL, '2026-05-26 12:00:00'),
(4002, 4001, 'Listening Section 2: Town Festival Guide', 'listening', 2, 8, 0, NULL, '2026-05-26 12:00:00'),
(4003, 4001, 'Listening Section 3: Dissertation Methodology', 'listening', 3, 10, 0, NULL, '2026-05-26 12:00:00'),
(4004, 4001, 'Listening Section 4: History of Public Transport', 'listening', 4, 10, 0, NULL, '2026-05-26 12:00:00'),
(4005, 4002, 'Reading Passage 1: Plastic Waste and Ocean Health', 'reading', 2, 20, 0, NULL, '2026-05-26 12:00:00'),
(4006, 4002, 'Reading Passage 2: The Science of Happiness', 'reading', 3, 20, 0, NULL, '2026-05-26 12:00:00'),
(4007, 4002, 'Reading Passage 3: Language Preservation Efforts', 'reading', 4, 20, 0, NULL, '2026-05-26 12:00:00'),
(4008, 4003, 'Writing Task 1: University Enrollment Trends', 'writing', 3, 20, 0, NULL, '2026-05-26 12:00:00'),
(4009, 4003, 'Writing Task 2: Technology and Privacy', 'writing', 3, 40, 0, NULL, '2026-05-26 12:00:00'),
(4010, 4004, 'Speaking Part 1: Work and Studies', 'speaking', 2, 5, 0, NULL, '2026-05-26 12:00:00'),
(4011, 4004, 'Speaking Part 2: Describe a Skill', 'speaking', 3, 4, 0, NULL, '2026-05-26 12:00:00'),
(4012, 4004, 'Speaking Part 3: Lifelong Learning', 'speaking', 4, 5, 0, NULL, '2026-05-26 12:00:00'),
-- Test 4 Lessons
(5001, 5001, 'Listening Section 1: Car Rental Enquiry', 'listening', 2, 8, 0, NULL, '2026-05-26 12:00:00'),
(5002, 5001, 'Listening Section 2: Campus Orientation', 'listening', 2, 8, 0, NULL, '2026-05-26 12:00:00'),
(5003, 5001, 'Listening Section 3: Marketing Project Discussion', 'listening', 3, 10, 0, NULL, '2026-05-26 12:00:00'),
(5004, 5001, 'Listening Section 4: Sustainable Architecture', 'listening', 4, 10, 0, NULL, '2026-05-26 12:00:00'),
(5005, 5002, 'Reading Passage 1: Cycling Infrastructure in Cities', 'reading', 2, 20, 0, NULL, '2026-05-26 12:00:00'),
(5006, 5002, 'Reading Passage 2: Neuroscience of Decision Making', 'reading', 3, 20, 0, NULL, '2026-05-26 12:00:00'),
(5007, 5002, 'Reading Passage 3: Ethics of Genetic Engineering', 'reading', 4, 20, 0, NULL, '2026-05-26 12:00:00'),
(5008, 5003, 'Writing Task 1: CO2 Emissions Comparison', 'writing', 3, 20, 0, NULL, '2026-05-26 12:00:00'),
(5009, 5003, 'Writing Task 2: Urbanisation Challenges', 'writing', 3, 40, 0, NULL, '2026-05-26 12:00:00'),
(5010, 5004, 'Speaking Part 1: Hobbies and Free Time', 'speaking', 2, 5, 0, NULL, '2026-05-26 12:00:00'),
(5011, 5004, 'Speaking Part 2: Describe an Achievement', 'speaking', 3, 4, 0, NULL, '2026-05-26 12:00:00'),
(5012, 5004, 'Speaking Part 3: Success and Motivation', 'speaking', 4, 5, 0, NULL, '2026-05-26 12:00:00');


--
-- Lesson Contents for Tests 2-4
--
INSERT INTO `tb_lesson_contents` (`content_id`, `lesson_id`, `content_type`, `content_body`) VALUES
-- Test 2
(3001, 3001, 'audio', 'Listen to a phone conversation about booking a hotel room for a conference.'),
(3002, 3002, 'audio', 'Listen to a librarian explain the services available at a local library.'),
(3003, 3003, 'audio', 'Three students discuss how to organise their group assignment.'),
(3004, 3004, 'audio', 'A university lecture on renewable energy sources and their economic impact.'),
(3005, 3005, 'article', 'Read the passage about urban farming initiatives and answer questions.'),
(3006, 3006, 'article', 'Read the passage about digital literacy programmes in schools.'),
(3007, 3007, 'article', 'Read the passage about behavioural economics and public policy.'),
(3008, 3008, 'exercise', 'Describe the chart showing water consumption by household type.'),
(3009, 3009, 'exercise', 'Write about the positive and negative effects of international tourism.'),
(3010, 3010, 'exercise', 'Answer questions about your neighbourhood and where you live.'),
(3011, 3011, 'exercise', 'Describe a book that had a significant impact on you.'),
(3012, 3012, 'exercise', 'Discuss reading habits and the role of books in modern society.'),
-- Test 3
(4001, 4001, 'audio', 'Listen to a phone call about registering for a sports centre membership.'),
(4002, 4002, 'audio', 'Listen to an organiser describe activities at a town festival.'),
(4003, 4003, 'audio', 'Two students and a supervisor discuss dissertation methodology.'),
(4004, 4004, 'audio', 'A lecture on the history and evolution of public transport systems.'),
(4005, 4005, 'article', 'Read the passage about plastic waste impact on ocean ecosystems.'),
(4006, 4006, 'article', 'Read the passage about psychological research into happiness.'),
(4007, 4007, 'article', 'Read the passage about efforts to preserve endangered languages.'),
(4008, 4008, 'exercise', 'Describe the line graph showing university enrollment trends by gender.'),
(4009, 4009, 'exercise', 'Write about the effects of technology on personal privacy.'),
(4010, 4010, 'exercise', 'Answer questions about your work, studies, and daily routine.'),
(4011, 4011, 'exercise', 'Describe a useful skill you learned outside of school or university.'),
(4012, 4012, 'exercise', 'Discuss the importance of lifelong learning in modern society.'),
-- Test 4
(5001, 5001, 'audio', 'Listen to a customer enquiring about car rental options and insurance.'),
(5002, 5002, 'audio', 'Listen to a university representative giving a campus orientation talk.'),
(5003, 5003, 'audio', 'Two students and a tutor discuss their marketing project approach.'),
(5004, 5004, 'audio', 'A lecture on sustainable architecture and green building design.'),
(5005, 5005, 'article', 'Read the passage about cycling infrastructure development in European cities.'),
(5006, 5006, 'article', 'Read the passage about how the brain makes decisions under uncertainty.'),
(5007, 5007, 'article', 'Read the passage about the ethical implications of genetic engineering.'),
(5008, 5008, 'exercise', 'Describe the bar chart comparing CO2 emissions across five countries.'),
(5009, 5009, 'exercise', 'Write about the challenges and benefits of rapid urbanisation.'),
(5010, 5010, 'exercise', 'Answer questions about your hobbies and how you spend free time.'),
(5011, 5011, 'exercise', 'Describe a personal achievement that you are proud of.'),
(5012, 5012, 'exercise', 'Discuss what motivates people to succeed and whether success can be taught.');


--
-- Tests for Cambridge 20 Tests 2-4
--
INSERT INTO `tb_tests` (`test_id`, `title`, `difficulty`, `duration_minutes`, `is_deleted`, `deleted_at`, `updated_at`, `created_by`, `is_teacher_created`, `is_public`) VALUES
-- Test 2
(3001, 'Cambridge 20 - Listening Test 2', 3, 30, 0, NULL, '2026-05-26 12:00:00', NULL, 0, 1),
(3002, 'Cambridge 20 - Reading Test 2', 3, 60, 0, NULL, '2026-05-26 12:00:00', NULL, 0, 1),
(3003, 'Cambridge 20 - Writing Test 2', 3, 60, 0, NULL, '2026-05-26 12:00:00', NULL, 0, 1),
(3004, 'Cambridge 20 - Speaking Test 2', 3, 14, 0, NULL, '2026-05-26 12:00:00', NULL, 0, 1),
(3005, 'Cambridge 20 - Full Academic Test 2', 4, 174, 0, NULL, '2026-05-26 12:00:00', NULL, 0, 1),
-- Test 3
(4001, 'Cambridge 20 - Listening Test 3', 3, 30, 0, NULL, '2026-05-26 12:00:00', NULL, 0, 1),
(4002, 'Cambridge 20 - Reading Test 3', 3, 60, 0, NULL, '2026-05-26 12:00:00', NULL, 0, 1),
(4003, 'Cambridge 20 - Writing Test 3', 3, 60, 0, NULL, '2026-05-26 12:00:00', NULL, 0, 1),
(4004, 'Cambridge 20 - Speaking Test 3', 3, 14, 0, NULL, '2026-05-26 12:00:00', NULL, 0, 1),
(4005, 'Cambridge 20 - Full Academic Test 3', 4, 174, 0, NULL, '2026-05-26 12:00:00', NULL, 0, 1),
-- Test 4
(5001, 'Cambridge 20 - Listening Test 4', 3, 30, 0, NULL, '2026-05-26 12:00:00', NULL, 0, 1),
(5002, 'Cambridge 20 - Reading Test 4', 3, 60, 0, NULL, '2026-05-26 12:00:00', NULL, 0, 1),
(5003, 'Cambridge 20 - Writing Test 4', 3, 60, 0, NULL, '2026-05-26 12:00:00', NULL, 0, 1),
(5004, 'Cambridge 20 - Speaking Test 4', 3, 14, 0, NULL, '2026-05-26 12:00:00', NULL, 0, 1),
(5005, 'Cambridge 20 - Full Academic Test 4', 4, 174, 0, NULL, '2026-05-26 12:00:00', NULL, 0, 1);


--
-- Test Sections for Cambridge 20 Tests 2-4
--
INSERT INTO `tb_test_sections` (`section_id`, `test_id`, `skill_type`, `audio_url`) VALUES
-- Test 2
(3001, 3001, 'listening', '/audio/cam20/T2S1.m4a'),
(3002, 3002, 'reading', NULL),
(3003, 3003, 'writing', NULL),
(3004, 3004, 'speaking', NULL),
(3005, 3005, 'listening', '/audio/cam20/T2S1.m4a'),
(3006, 3005, 'reading', NULL),
(3007, 3005, 'writing', NULL),
(3008, 3005, 'speaking', NULL),
-- Test 3
(4001, 4001, 'listening', '/audio/cam20/T3S1.m4a'),
(4002, 4002, 'reading', NULL),
(4003, 4003, 'writing', NULL),
(4004, 4004, 'speaking', NULL),
(4005, 4005, 'listening', '/audio/cam20/T3S1.m4a'),
(4006, 4005, 'reading', NULL),
(4007, 4005, 'writing', NULL),
(4008, 4005, 'speaking', NULL),
-- Test 4
(5001, 5001, 'listening', '/audio/cam20/T4S1.m4a'),
(5002, 5002, 'reading', NULL),
(5003, 5003, 'writing', NULL),
(5004, 5004, 'speaking', NULL),
(5005, 5005, 'listening', '/audio/cam20/T4S1.m4a'),
(5006, 5005, 'reading', NULL),
(5007, 5005, 'writing', NULL),
(5008, 5005, 'speaking', NULL);


--
-- Listening Materials for Tests 2-4
--
INSERT INTO `tb_listening_materials` (`material_id`, `lesson_id`, `material_type`, `title`, `transcript`, `audio_url`, `duration_seconds`, `speaker_count`, `topics`, `notes_for_learner`, `difficulty_level`, `source`, `created_at`) VALUES
-- Test 2
(3001, 3001, 'conversation', 'Hotel Conference Booking', 'Receptionist: Grand Oak Hotel, good morning.\nCaller: Hello, I\'d like to book a room for a three-day conference.\nReceptionist: Certainly. What dates are you looking at?\nCaller: The twenty-third to the twenty-fifth of October.\nReceptionist: Let me check availability. We have a standard room at seventy-five pounds per night or a deluxe suite at one hundred and twenty.\nCaller: I\'ll take the standard, please. My name is James Whitfield. W-H-I-T-F-I-E-L-D.\nReceptionist: And a contact number?\nCaller: It\'s oh-seven-seven-zero, four-five-three, eight-nine-one-two.\nReceptionist: The conference centre is on the second floor. Registration opens at eight thirty AM. Breakfast is included and served in the Garden Restaurant from six to nine.\nCaller: Is there parking?\nReceptionist: Yes, the underground car park is free for guests. Enter from Victoria Street.', '/audio/cam20/T2S1.m4a', 300, 2, 'hotel,booking,conference', 'Listen for corrected numbers, spelling, and specific details.', 'band_5_6', 'Cambridge IELTS 20 Academic', '2026-05-26 12:00:00'),
(3002, 3002, 'monologue', 'Public Library Services Overview', 'Good afternoon everyone. Welcome to Greenfield Public Library. I\'m going to tell you about the services we offer. On the ground floor, you\'ll find our main collection of over forty thousand books. The children\'s section is in the east wing, which was renovated last March. We have twelve computer stations available on a first-come basis, with a maximum booking time of two hours. Our meeting rooms on the first floor can be reserved for community groups — the large one seats up to thirty people. We run free digital skills workshops every Wednesday afternoon from two to four. The café is located next to the main entrance, and it closes at five thirty. We also have a home delivery service for elderly or disabled members who cannot visit in person.', '/audio/cam20/T2S2.m4a', 330, 1, 'library,services,community', 'Track numbers, times, and locations carefully.', 'band_5_6', 'Cambridge IELTS 20 Academic', '2026-05-26 12:00:00'),
(3003, 3003, 'discussion', 'Group Assignment: Tourism Impact Study', 'Tom: So we need to decide our research focus for the tourism impact assignment.\nLisa: I think we should look at economic effects on local businesses.\nTom: But Dr Chen said environmental impact is more relevant to the module.\nMia: Why not combine both? We could compare economic benefits against environmental costs.\nTom: That might be too broad for three thousand words.\nLisa: What about focusing on one destination? Like the Lake District.\nMia: Good idea. We could use the council\'s annual visitor statistics.\nTom: I\'ll handle the data analysis. Lisa, could you do the literature review?\nLisa: Sure. And Mia?\nMia: I\'ll write the methodology section and design the questionnaire.\nTom: We should meet again next Thursday. The draft is due on the fifteenth of November.\nLisa: Don\'t forget we need to submit the ethics form by Friday.', '/audio/cam20/T2S3.m4a', 380, 3, 'tourism,research,assignment', 'Follow speaker opinions and task allocations carefully.', 'band_6_7', 'Cambridge IELTS 20 Academic', '2026-05-26 12:00:00'),
(3004, 3004, 'lecture', 'Renewable Energy: Progress and Challenges', 'In today\'s lecture, I want to examine the global progress in renewable energy adoption. Solar power capacity has increased by approximately four hundred percent over the past decade, with China leading installations at roughly thirty-five percent of global capacity. Wind energy has also expanded significantly, now supplying about seven percent of the world\'s electricity. However, the transition faces three major challenges. First, intermittency: solar panels produce nothing at night, and wind turbines require consistent wind speeds above twelve kilometres per hour. Second, storage technology remains expensive — lithium-ion batteries currently cost around one hundred and thirty-two dollars per kilowatt-hour, though this has fallen from over a thousand dollars in 2010. Third, grid infrastructure in many developing nations cannot accommodate distributed generation. The International Energy Agency projects that renewables will constitute forty-five percent of global electricity by 2040, but only if annual investment reaches approximately four trillion dollars.', '/audio/cam20/T2S4.m4a', 420, 1, 'energy,environment,economics', 'Note statistics, percentages, and technical terminology.', 'band_6_7', 'Cambridge IELTS 20 Academic', '2026-05-26 12:00:00'),
-- Test 3
(4001, 4001, 'conversation', 'Sports Centre Membership', 'Staff: Lakeside Sports Centre, how can I help?\nCaller: I\'d like to join as a member. What options do you have?\nStaff: We offer three packages. Basic is thirty pounds monthly — gym and pool access. Premium is fifty-five — that includes all classes, sauna, and two guest passes per month. Elite is eighty and adds personal training sessions.\nCaller: I\'ll go with Premium. My name is Rebecca Morrison. M-O-R-R-I-S-O-N.\nStaff: Date of birth?\nCaller: Seventh of August, nineteen eighty-eight.\nStaff: The pool opens at six AM and closes at nine PM. We\'re closed on bank holidays. Your membership card will be ready for collection on Monday.\nCaller: Is there a joining fee?\nStaff: There\'s a one-time fee of fifteen pounds for the membership card. Classes need to be booked twenty-four hours in advance through our app.', '/audio/cam20/T3S1.m4a', 300, 2, 'fitness,membership,services', 'Listen for prices, times, names, and conditions.', 'band_5_6', 'Cambridge IELTS 20 Academic', '2026-05-26 12:00:00'),
(4002, 4002, 'monologue', 'Annual Riverside Festival Information', 'Welcome to the thirty-second Riverside Festival. Let me give you some practical information. The festival runs from Friday the eighth to Sunday the tenth of June. The main stage is in Victoria Park, opposite the town hall. Food stalls are along the riverbank — we have over twenty vendors this year, including a dedicated vegetarian area near the bridge. The craft market is in the old warehouse on Mill Street, open from ten AM to six PM. Children\'s activities are in the Community Centre car park, including face painting and a puppet show at three PM each day. Music performances begin at noon and finish at ten PM. Please note that no glass bottles are permitted in the park area. Toilets are located behind the information tent and near the east car park.', '/audio/cam20/T3S2.m4a', 330, 1, 'festival,events,directions', 'Map locations and times to the correct features.', 'band_5_6', 'Cambridge IELTS 20 Academic', '2026-05-26 12:00:00'),
(4003, 4003, 'discussion', 'Dissertation: Survey Design Discussion', 'Dr Harris: How is your dissertation survey coming along, James?\nJames: We\'ve drafted fifteen questions, but I\'m not sure about the response scale.\nSophie: I think a five-point Likert scale would work better than seven. It\'s less confusing for participants.\nDr Harris: That\'s a common approach. What about your pilot study?\nJames: We plan to test it with twenty participants from the second-year cohort.\nSophie: I suggested thirty, but James thinks twenty is enough for a pilot.\nDr Harris: Twenty is acceptable for identifying major issues. When do you plan to run it?\nJames: Next Monday. The full survey launches on the first of December.\nSophie: We also need to decide on distribution — online only, or include paper copies?\nDr Harris: I\'d recommend both. Not everyone checks university email regularly.\nJames: Good point. We\'ll add QR codes to posters in the library and student union.', '/audio/cam20/T3S3.m4a', 380, 3, 'research,survey,methodology', 'Match speaker opinions and note methodological decisions.', 'band_6_7', 'Cambridge IELTS 20 Academic', '2026-05-26 12:00:00'),
(4004, 4004, 'lecture', 'The Evolution of Urban Public Transport', 'The history of urban public transport tells us much about how cities have developed. The first horse-drawn omnibus service began in Paris in eighteen twenty-eight, carrying up to sixteen passengers along fixed routes. London followed in eighteen twenty-nine. By the eighteen-sixties, underground railways emerged — the Metropolitan Railway opened in London in January eighteen sixty-three, carrying thirty-eight thousand passengers on its first day. Electric trams appeared in the eighteen-eighties, initially in Germany, and quickly spread across European and American cities. The twentieth century brought motor buses and, eventually, light rail systems. Today, approximately fifty-five percent of the world\'s population lives in urban areas, and this is projected to reach sixty-eight percent by twenty-fifty. The challenge now is reducing carbon emissions while maintaining mobility. Cities like Bogotá have pioneered Bus Rapid Transit systems, which can carry up to forty-five thousand passengers per hour at a fraction of the cost of underground rail.', '/audio/cam20/T3S4.m4a', 420, 1, 'transport,history,urbanisation', 'Pay attention to dates, numbers, and sequence of events.', 'band_6_7', 'Cambridge IELTS 20 Academic', '2026-05-26 12:00:00'),
-- Test 4
(5001, 5001, 'conversation', 'Car Rental Enquiry', 'Agent: Sunrise Car Rentals, good morning.\nCustomer: Hello, I need to hire a car for next weekend.\nAgent: Certainly. We have compact cars from thirty-five pounds per day, or SUVs from fifty-five.\nCustomer: A compact will be fine. From Saturday to Monday.\nAgent: Three days, that\'s a hundred and five pounds total. Would you like our collision damage waiver? It\'s eight pounds fifty per day.\nCustomer: Yes, please. My name is Oliver Cheng. C-H-E-N-G.\nAgent: And your driving licence number?\nCustomer: It\'s UK-seven-nine-four-two-one-eight.\nAgent: Collection is from our airport branch, Terminal Two. We open at seven AM. You\'ll need to bring your licence and a credit card for the deposit — that\'s two hundred pounds, refundable on return.\nCustomer: What about fuel?\nAgent: The tank will be full when you collect. Please return it full, otherwise there\'s a surcharge of one pound eighty per litre.', '/audio/cam20/T4S1.m4a', 300, 2, 'car-rental,travel,booking', 'Listen carefully for prices, reference numbers, and conditions.', 'band_5_6', 'Cambridge IELTS 20 Academic', '2026-05-26 12:00:00'),
(5002, 5002, 'monologue', 'University Campus Orientation', 'Good morning, new students. Welcome to Westfield University. Let me walk you through the campus layout. We\'re standing in the Main Quad. To your left is the Humanities Building, which houses the English, History, and Philosophy departments. Straight ahead is the Science Complex — that was completed in twenty twenty-two at a cost of eighteen million pounds. The Student Union is behind us, next to the sports hall. The library is our largest building, with over six hundred thousand volumes across five floors. It\'s open twenty-four hours during exam periods. The health centre is on the north side of campus, beside the lake. International students should visit the Support Office on the second floor of the Administration Building to collect their student cards. Car parking permits can be applied for online — there are four hundred spaces available on a first-come basis.', '/audio/cam20/T4S2.m4a', 330, 1, 'university,campus,orientation', 'Note building locations, numbers, and services mentioned.', 'band_5_6', 'Cambridge IELTS 20 Academic', '2026-05-26 12:00:00'),
(5003, 5003, 'discussion', 'Marketing Project: Brand Analysis', 'Professor Lee: How are you progressing with the brand analysis project?\nAlex: We\'ve chosen to study a sustainable fashion brand called EcoThread.\nKira: Their revenue grew by sixty percent last year, mainly through social media marketing.\nProfessor Lee: Interesting. What theoretical framework are you using?\nAlex: We\'re applying Porter\'s Five Forces combined with SWOT analysis.\nKira: I wanted to use the Marketing Mix 7Ps approach instead.\nProfessor Lee: Why not integrate both? The 7Ps for internal analysis and Porter\'s for external.\nAlex: That could work. Our presentation is on the twelfth of December.\nKira: We still need to conduct three interviews with industry professionals.\nProfessor Lee: Make sure you record them and get signed consent forms. Also, check the company\'s annual report for twenty twenty-four — it was published last month.\nAlex: We found it on their investor relations page. The key challenge they mention is supply chain transparency.', '/audio/cam20/T4S3.m4a', 380, 3, 'marketing,business,analysis', 'Follow the academic discussion and note each speaker\'s position.', 'band_6_7', 'Cambridge IELTS 20 Academic', '2026-05-26 12:00:00'),
(5004, 5004, 'lecture', 'Green Building Design and Sustainable Architecture', 'Sustainable architecture has moved from a niche concern to a mainstream requirement in modern construction. The built environment accounts for approximately thirty-nine percent of global carbon emissions — twenty-eight percent from building operations and eleven percent from materials and construction. Green building standards such as BREEAM in the UK and LEED in North America provide measurable benchmarks. Key strategies include passive solar design, which can reduce heating costs by up to seventy percent, and green roofs that lower urban temperatures by two to three degrees Celsius. Cross-laminated timber is emerging as a viable alternative to concrete, storing roughly one tonne of CO2 per cubic metre. The Bullitt Center in Seattle, often called the greenest commercial building in the world, generates more electricity than it consumes through rooftop solar panels and uses composting toilets to eliminate sewage output. However, the cost premium for green construction remains between five and fifteen percent, which deters many developers despite long-term operational savings.', '/audio/cam20/T4S4.m4a', 420, 1, 'architecture,sustainability,environment', 'Note percentages, technical terms, and examples.', 'band_6_7', 'Cambridge IELTS 20 Academic', '2026-05-26 12:00:00');


--
-- Listening Questions for Test 2
--
INSERT INTO `tb_listening_questions` (`question_id`, `material_id`, `question_type`, `question_text`, `time_code_start`, `time_code_end`, `correct_answer`, `options`, `explanation`, `band_target`, `question_order`, `created_at`) VALUES
-- Test 2 Section 1
(3001, 3001, 'form_completion', 'Conference dates: 23rd to 25th ____', 10, 25, 'October', NULL, 'Caller states October clearly.', 5.5, 1, '2026-05-26 12:00:00'),
(3002, 3001, 'form_completion', 'Standard room cost per night: £____', 28, 40, '75', NULL, 'Seventy-five pounds per night.', 5.5, 2, '2026-05-26 12:00:00'),
(3003, 3001, 'form_completion', 'Guest surname spelling: ____', 42, 55, 'Whitfield', NULL, 'Spelled W-H-I-T-F-I-E-L-D.', 5.5, 3, '2026-05-26 12:00:00'),
(3004, 3001, 'form_completion', 'Registration opens at ____', 58, 70, '8:30', NULL, 'Eight thirty AM.', 6.0, 4, '2026-05-26 12:00:00'),
(3005, 3001, 'form_completion', 'Car park entrance is from ____ Street.', 72, 85, 'Victoria', NULL, 'Enter from Victoria Street.', 6.0, 5, '2026-05-26 12:00:00'),
-- Test 2 Section 2
(3006, 3002, 'note_completion', 'Number of books in collection: over ____', 15, 28, '40,000', NULL, 'Over forty thousand books.', 6.0, 6, '2026-05-26 12:00:00'),
(3007, 3002, 'note_completion', 'Children''s section renovated in ____.', 30, 42, 'March', NULL, 'Renovated last March.', 6.0, 7, '2026-05-26 12:00:00'),
(3008, 3002, 'form_completion', 'Maximum computer booking time: ____ hours', 44, 56, '2', NULL, 'Maximum two hours.', 6.0, 8, '2026-05-26 12:00:00'),
(3009, 3002, 'note_completion', 'Large meeting room capacity: ____ people', 58, 70, '30', NULL, 'Seats up to thirty.', 6.0, 9, '2026-05-26 12:00:00'),
(3010, 3002, 'note_completion', 'Digital skills workshops are on ____ afternoons.', 72, 85, 'Wednesday', NULL, 'Every Wednesday afternoon.', 5.5, 10, '2026-05-26 12:00:00'),
-- Test 2 Section 3
(3011, 3003, 'multiple_choice', 'What research focus does Lisa suggest?', 18, 32, 'A', '["A. Economic effects on local businesses","B. Environmental impact on wildlife","C. Social effects on residents","D. Infrastructure development"]', 'Lisa suggests economic effects.', 6.5, 11, '2026-05-26 12:00:00'),
(3012, 3003, 'multiple_choice', 'Which destination do they choose?', 34, 48, 'C', '["A. Peak District","B. Cornwall","C. Lake District","D. Cotswolds"]', 'They agree on the Lake District.', 6.5, 12, '2026-05-26 12:00:00'),
(3013, 3003, 'matching', 'Tom will be responsible for ____.', 50, 62, 'data analysis', NULL, 'Tom volunteers for data analysis.', 6.5, 13, '2026-05-26 12:00:00'),
(3014, 3003, 'matching', 'Lisa will handle the ____.', 62, 74, 'literature review', NULL, 'Lisa agrees to do the literature review.', 6.5, 14, '2026-05-26 12:00:00'),
(3015, 3003, 'form_completion', 'Draft deadline: 15th ____.', 76, 88, 'November', NULL, 'Due on the fifteenth of November.', 6.5, 15, '2026-05-26 12:00:00'),
-- Test 2 Section 4
(3016, 3004, 'note_completion', 'Solar capacity increase over past decade: ____%.', 14, 28, '400', NULL, 'Approximately four hundred percent.', 7.0, 16, '2026-05-26 12:00:00'),
(3017, 3004, 'note_completion', 'China''s share of global solar installations: ____%.', 30, 44, '35', NULL, 'Roughly thirty-five percent.', 7.0, 17, '2026-05-26 12:00:00'),
(3018, 3004, 'summary_completion', 'Wind turbines require speeds above ____ km/h.', 46, 60, '12', NULL, 'Twelve kilometres per hour.', 7.0, 18, '2026-05-26 12:00:00'),
(3019, 3004, 'note_completion', 'Current battery cost: $____ per kWh.', 62, 76, '132', NULL, 'One hundred and thirty-two dollars.', 7.0, 19, '2026-05-26 12:00:00'),
(3020, 3004, 'summary_completion', 'Renewables projected to be ____% of electricity by 2040.', 78, 95, '45', NULL, 'Forty-five percent by 2040.', 7.0, 20, '2026-05-26 12:00:00');


--
-- Listening Questions for Test 3
--
INSERT INTO `tb_listening_questions` (`question_id`, `material_id`, `question_type`, `question_text`, `time_code_start`, `time_code_end`, `correct_answer`, `options`, `explanation`, `band_target`, `question_order`, `created_at`) VALUES
-- Test 3 Section 1
(4001, 4001, 'form_completion', 'Premium membership cost: £____ per month', 10, 25, '55', NULL, 'Fifty-five pounds monthly.', 5.5, 1, '2026-05-26 12:00:00'),
(4002, 4001, 'form_completion', 'Member surname: ____', 28, 40, 'Morrison', NULL, 'Spelled M-O-R-R-I-S-O-N.', 5.5, 2, '2026-05-26 12:00:00'),
(4003, 4001, 'form_completion', 'Date of birth: 7th August ____', 42, 55, '1988', NULL, 'Nineteen eighty-eight.', 5.5, 3, '2026-05-26 12:00:00'),
(4004, 4001, 'form_completion', 'Pool closing time: ____', 58, 68, '9 PM', NULL, 'Closes at nine PM.', 6.0, 4, '2026-05-26 12:00:00'),
(4005, 4001, 'form_completion', 'Joining fee for membership card: £____', 70, 82, '15', NULL, 'One-time fee of fifteen pounds.', 6.0, 5, '2026-05-26 12:00:00'),
-- Test 3 Section 2
(4006, 4002, 'note_completion', 'Festival runs from 8th to 10th ____.', 15, 28, 'June', NULL, 'Eighth to tenth of June.', 6.0, 6, '2026-05-26 12:00:00'),
(4007, 4002, 'note_completion', 'Number of food vendors: over ____.', 30, 42, '20', NULL, 'Over twenty vendors.', 6.0, 7, '2026-05-26 12:00:00'),
(4008, 4002, 'note_completion', 'Craft market located on ____ Street.', 44, 56, 'Mill', NULL, 'Old warehouse on Mill Street.', 6.0, 8, '2026-05-26 12:00:00'),
(4009, 4002, 'note_completion', 'Puppet show starts at ____ PM daily.', 58, 68, '3', NULL, 'Three PM each day.', 5.5, 9, '2026-05-26 12:00:00'),
(4010, 4002, 'multiple_choice', 'What is NOT allowed in the park?', 70, 82, 'B', '["A. Plastic bottles","B. Glass bottles","C. Pets","D. Bicycles"]', 'No glass bottles permitted.', 6.0, 10, '2026-05-26 12:00:00'),
-- Test 3 Section 3
(4011, 4003, 'multiple_choice', 'What response scale does Sophie prefer?', 18, 32, 'A', '["A. Five-point Likert","B. Seven-point Likert","C. Binary yes/no","D. Open-ended"]', 'Sophie prefers five-point.', 6.5, 11, '2026-05-26 12:00:00'),
(4012, 4003, 'form_completion', 'Pilot study participants: ____', 34, 48, '20', NULL, 'Twenty participants for pilot.', 6.5, 12, '2026-05-26 12:00:00'),
(4013, 4003, 'form_completion', 'Full survey launch date: 1st ____.', 50, 62, 'December', NULL, 'First of December.', 6.5, 13, '2026-05-26 12:00:00'),
(4014, 4003, 'multiple_choice', 'What distribution method does Dr Harris recommend?', 64, 78, 'C', '["A. Online only","B. Paper only","C. Both online and paper","D. Email only"]', 'Both methods recommended.', 6.5, 14, '2026-05-26 12:00:00'),
(4015, 4003, 'note_completion', 'QR codes will be placed in the ____ and student union.', 80, 95, 'library', NULL, 'Library and student union.', 6.5, 15, '2026-05-26 12:00:00'),
-- Test 3 Section 4
(4016, 4004, 'note_completion', 'First horse-drawn omnibus: Paris, ____.', 14, 28, '1828', NULL, 'Eighteen twenty-eight.', 7.0, 16, '2026-05-26 12:00:00'),
(4017, 4004, 'note_completion', 'Metropolitan Railway first-day passengers: ____', 30, 44, '38,000', NULL, 'Thirty-eight thousand passengers.', 7.0, 17, '2026-05-26 12:00:00'),
(4018, 4004, 'note_completion', 'Electric trams first appeared in ____.', 46, 60, 'Germany', NULL, 'Initially in Germany.', 7.0, 18, '2026-05-26 12:00:00'),
(4019, 4004, 'summary_completion', 'Urban population projected to reach ____% by 2050.', 62, 76, '68', NULL, 'Sixty-eight percent by 2050.', 7.0, 19, '2026-05-26 12:00:00'),
(4020, 4004, 'note_completion', 'BRT capacity: up to ____ passengers per hour.', 78, 95, '45,000', NULL, 'Forty-five thousand passengers.', 7.0, 20, '2026-05-26 12:00:00');


--
-- Listening Questions for Test 4
--
INSERT INTO `tb_listening_questions` (`question_id`, `material_id`, `question_type`, `question_text`, `time_code_start`, `time_code_end`, `correct_answer`, `options`, `explanation`, `band_target`, `question_order`, `created_at`) VALUES
-- Test 4 Section 1
(5001, 5001, 'form_completion', 'Compact car daily rate: £____', 10, 22, '35', NULL, 'Thirty-five pounds per day.', 5.5, 1, '2026-05-26 12:00:00'),
(5002, 5001, 'form_completion', 'Total rental cost for 3 days: £____', 24, 36, '105', NULL, 'One hundred and five pounds total.', 5.5, 2, '2026-05-26 12:00:00'),
(5003, 5001, 'form_completion', 'Collision damage waiver: £____ per day', 38, 50, '8.50', NULL, 'Eight pounds fifty per day.', 6.0, 3, '2026-05-26 12:00:00'),
(5004, 5001, 'form_completion', 'Customer surname: ____', 52, 64, 'Cheng', NULL, 'Spelled C-H-E-N-G.', 5.5, 4, '2026-05-26 12:00:00'),
(5005, 5001, 'form_completion', 'Refundable deposit amount: £____', 66, 78, '200', NULL, 'Two hundred pounds deposit.', 6.0, 5, '2026-05-26 12:00:00'),
-- Test 4 Section 2
(5006, 5002, 'note_completion', 'Science Complex completed in ____.', 15, 28, '2022', NULL, 'Completed in twenty twenty-two.', 6.0, 6, '2026-05-26 12:00:00'),
(5007, 5002, 'note_completion', 'Library holds over ____ volumes.', 30, 42, '600,000', NULL, 'Over six hundred thousand volumes.', 6.0, 7, '2026-05-26 12:00:00'),
(5008, 5002, 'note_completion', 'Library is open ____ hours during exams.', 44, 56, '24', NULL, 'Twenty-four hours during exam periods.', 6.0, 8, '2026-05-26 12:00:00'),
(5009, 5002, 'note_completion', 'Health centre is beside the ____.', 58, 68, 'lake', NULL, 'North side of campus beside the lake.', 5.5, 9, '2026-05-26 12:00:00'),
(5010, 5002, 'form_completion', 'Total car parking spaces: ____', 70, 82, '400', NULL, 'Four hundred spaces.', 6.0, 10, '2026-05-26 12:00:00'),
-- Test 4 Section 3
(5011, 5003, 'form_completion', 'Brand studied: ____', 18, 30, 'EcoThread', NULL, 'Sustainable fashion brand called EcoThread.', 6.5, 11, '2026-05-26 12:00:00'),
(5012, 5003, 'note_completion', 'Revenue growth last year: ____%.', 32, 44, '60', NULL, 'Grew by sixty percent.', 6.5, 12, '2026-05-26 12:00:00'),
(5013, 5003, 'multiple_choice', 'Which framework does Kira prefer?', 46, 58, 'B', '["A. Porter''s Five Forces","B. Marketing Mix 7Ps","C. BCG Matrix","D. PESTLE"]', 'Kira prefers 7Ps approach.', 6.5, 13, '2026-05-26 12:00:00'),
(5014, 5003, 'form_completion', 'Presentation date: 12th ____.', 60, 72, 'December', NULL, 'Twelfth of December.', 6.5, 14, '2026-05-26 12:00:00'),
(5015, 5003, 'note_completion', 'Key challenge mentioned in annual report: supply chain ____.', 74, 88, 'transparency', NULL, 'Supply chain transparency.', 6.5, 15, '2026-05-26 12:00:00'),
-- Test 4 Section 4
(5016, 5004, 'note_completion', 'Built environment accounts for ____% of global emissions.', 14, 28, '39', NULL, 'Approximately thirty-nine percent.', 7.0, 16, '2026-05-26 12:00:00'),
(5017, 5004, 'summary_completion', 'Passive solar design reduces heating costs by up to ____%.', 30, 44, '70', NULL, 'Up to seventy percent.', 7.0, 17, '2026-05-26 12:00:00'),
(5018, 5004, 'note_completion', 'Green roofs lower urban temperatures by ____ degrees.', 46, 58, '2 to 3', NULL, 'Two to three degrees Celsius.', 7.0, 18, '2026-05-26 12:00:00'),
(5019, 5004, 'note_completion', 'Cross-laminated timber stores ____ tonne(s) CO2 per cubic metre.', 60, 72, '1', NULL, 'Roughly one tonne per cubic metre.', 7.0, 19, '2026-05-26 12:00:00'),
(5020, 5004, 'summary_completion', 'Green construction cost premium: ____% to 15%.', 74, 90, '5', NULL, 'Between five and fifteen percent.', 7.0, 20, '2026-05-26 12:00:00');


--
-- Reading Passages for Tests 2-4
--
INSERT INTO `tb_reading_passages` (`passage_id`, `lesson_id`, `passage_title`, `passage_text`, `word_count`, `difficulty_level`, `topic_category`, `source`, `created_at`, `image_url`, `passage_translation`, `vocab_highlights`) VALUES
-- Test 2
(3001, 3005, 'The Rise of Urban Farming', 'Paragraph A: In cities around the world, a quiet revolution is taking place on rooftops, in vacant lots and inside converted warehouses. Urban farming — the practice of cultivating food within city boundaries — has grown from a fringe hobby into a serious response to food security challenges. A 2022 report by the Food and Agriculture Organization estimated that urban agriculture already supplies food to around eight hundred million people globally.\n\nParagraph B: The methods vary enormously. Rooftop gardens in Singapore produce leafy vegetables using hydroponic systems that consume ninety percent less water than conventional farming. Vertical farms in the Netherlands stack growing trays in climate-controlled towers, achieving yields up to three hundred and fifty times greater per square metre than open-field agriculture. Community allotments in Detroit have transformed thousands of empty plots into productive green spaces.\n\nParagraph C: Proponents argue that urban farming reduces food miles, lowers carbon emissions from transport, and strengthens community bonds. A study by the University of Sheffield found that community gardeners consumed forty percent more fruit and vegetables than non-gardeners. There are also mental health benefits: participants report reduced stress and greater social connectedness.\n\nParagraph D: Critics, however, point to limitations. Urban land is expensive, and the scale of production rarely matches that of rural agriculture. Contaminated soil in former industrial areas can pose health risks unless raised beds or enclosed systems are used. Furthermore, the energy costs of indoor vertical farming — particularly for artificial lighting — can offset the environmental gains from reduced transport.', 280, 'band_5_6', 'agriculture and urban planning', 'Cambridge IELTS 20 style', '2026-05-26 12:00:00', 'https://images.unsplash.com/photo-1530836369250-ef72a3f5cda8?q=80&w=1200', 'Paragraph A: Tại các thành phố trên toàn thế giới, một cuộc cách mạng thầm lặng đang diễn ra trên mái nhà, trên các mảnh đất trống và bên trong các nhà kho được cải tạo. Nông nghiệp đô thị — việc trồng trọt thực phẩm trong ranh giới thành phố — đã phát triển từ một sở thích bên lề thành một giải pháp nghiêm túc cho các thách thức an ninh lương thực.\n\nParagraph B: Các phương pháp rất đa dạng. Vườn trên mái ở Singapore sản xuất rau lá xanh bằng hệ thống thủy canh tiêu thụ ít hơn 90% nước so với canh tác thông thường. Trang trại thẳng đứng ở Hà Lan xếp chồng các khay trồng trong tháp kiểm soát khí hậu.\n\nParagraph C: Những người ủng hộ cho rằng nông nghiệp đô thị giảm quãng đường vận chuyển thực phẩm, giảm lượng khí thải carbon từ giao thông và tăng cường sự gắn kết cộng đồng.\n\nParagraph D: Tuy nhiên, các nhà phê bình chỉ ra những hạn chế. Đất đô thị đắt đỏ, và quy mô sản xuất hiếm khi sánh được với nông nghiệp nông thôn.', '[{"word":"hydroponic","ipa":"/ˌhaɪ.drəˈpɒn.ɪk/","vi":"thủy canh","en":"growing plants in water without soil"},{"word":"allotments","ipa":"/əˈlɒt.mənts/","vi":"mảnh đất chia nhỏ","en":"small plots of land rented for growing food"},{"word":"proponents","ipa":"/prəˈpəʊ.nənts/","vi":"người ủng hộ","en":"people who advocate for something"},{"word":"contaminated","ipa":"/kənˈtæm.ɪ.neɪ.tɪd/","vi":"bị ô nhiễm","en":"made impure or poisonous"},{"word":"offset","ipa":"/ˌɒf.ˈset/","vi":"bù đắp, cân bằng","en":"to counterbalance or compensate for"}]'),
-- Test 2 Passage 2
(3002, 3006, 'Digital Literacy and the Modern Classroom', 'Paragraph A: The concept of literacy has expanded dramatically in the twenty-first century. Where once it referred simply to the ability to read and write, educators now recognise digital literacy as an equally essential competency. Digital literacy encompasses the skills needed to find, evaluate, create and communicate information using digital technologies — skills that are increasingly demanded by employers across all sectors.\n\nParagraph B: A 2023 survey by the OECD found that only fifty-two percent of fifteen-year-olds across member countries could reliably distinguish between fact and opinion in online texts. This finding has prompted calls for systematic digital literacy education beginning in primary school. Finland has led the way, integrating critical media evaluation into its national curriculum since 2016.\n\nParagraph C: The challenge for educators is that digital literacy is not a single skill but a spectrum. At the basic level, students must learn to use devices and software. At the intermediate level, they need to evaluate source credibility, understand algorithmic bias, and protect their personal data. Advanced digital literacy involves creating content, understanding coding fundamentals, and participating responsibly in online communities.\n\nParagraph D: Opposition to mandatory digital literacy programmes centres on two concerns: screen time and cost. Critics worry that more technology in classrooms increases children''s already excessive screen exposure. Others note that equipping schools with up-to-date hardware requires significant investment — an average of twelve thousand pounds per classroom according to a recent UK estimate. Advocates counter that digital illiteracy will prove far more costly to society in the long term.', 300, 'band_6_7', 'education and technology', 'Cambridge IELTS 20 style', '2026-05-26 12:00:00', 'https://images.unsplash.com/photo-1509062522246-3755977927d7?q=80&w=1200', NULL, '[{"word":"encompass","ipa":"/ɪnˈkʌm.pəs/","vi":"bao gồm","en":"to include or contain"},{"word":"algorithmic","ipa":"/ˌæl.ɡəˈrɪð.mɪk/","vi":"thuộc thuật toán","en":"relating to algorithms or automated processes"},{"word":"credibility","ipa":"/ˌkred.ɪˈbɪl.ə.ti/","vi":"độ tin cậy","en":"the quality of being trusted and believed in"},{"word":"mandatory","ipa":"/ˈmæn.də.tər.i/","vi":"bắt buộc","en":"required by law or rules"},{"word":"illiteracy","ipa":"/ɪˈlɪt.ər.ə.si/","vi":"sự mù chữ","en":"inability to read or write, or lack of knowledge in a field"}]'),
-- Test 2 Passage 3
(3003, 3007, 'Behavioural Economics: Nudging Better Decisions', 'Paragraph A: Traditional economic theory assumes that humans are rational agents who consistently make decisions that maximise their self-interest. Behavioural economics challenges this assumption by demonstrating that people frequently rely on mental shortcuts, or heuristics, that lead to systematic errors in judgement. The field gained mainstream recognition when Daniel Kahneman received the Nobel Prize in Economics in 2002.\n\nParagraph B: One of the most influential applications of behavioural economics is the concept of "nudging" — designing choice environments so that people are more likely to make beneficial decisions without restricting their freedom. A landmark study in the United Kingdom found that changing the default pension enrollment from opt-in to opt-out increased participation rates from sixty-one percent to ninety-three percent within three years.\n\nParagraph C: Nudges have since been applied to health, energy conservation and tax compliance. In Guatemala, personalising tax reminder letters with the recipient''s name and a local social norm — "nine out of ten of your neighbours have already paid" — increased on-time payments by fifteen percent. Similarly, repositioning fruit to eye level in school cafeterias boosted consumption by twenty-five percent compared to placing it in less visible locations.\n\nParagraph D: Critics of nudging raise ethical questions about manipulation and paternalism. Who decides what constitutes a "better" decision? Libertarian scholars argue that even well-intentioned nudges undermine individual autonomy. There are also concerns about transparency — many nudge interventions are effective precisely because people are unaware of them. Proponents respond that all choice environments are designed, and that failing to consider behavioural insights simply means accepting a poorly designed default.', 320, 'band_6_7', 'economics and psychology', 'Cambridge IELTS 20 style', '2026-05-26 12:00:00', NULL, NULL, '[{"word":"heuristics","ipa":"/hjʊˈrɪs.tɪks/","vi":"phương pháp suy nghiệm","en":"mental shortcuts used to make quick judgments"},{"word":"nudging","ipa":"/ˈnʌdʒ.ɪŋ/","vi":"thúc đẩy nhẹ","en":"guiding someone toward a decision through indirect suggestions"},{"word":"paternalism","ipa":"/pəˈtɜː.nəl.ɪ.zəm/","vi":"chủ nghĩa gia trưởng","en":"limiting freedom for the supposed benefit of others"},{"word":"autonomy","ipa":"/ɔːˈtɒn.ə.mi/","vi":"quyền tự chủ","en":"the right or condition of self-governance"},{"word":"libertarian","ipa":"/ˌlɪb.əˈteə.ri.ən/","vi":"theo chủ nghĩa tự do","en":"advocating minimal state intervention in personal freedom"}]'),
-- Test 3
(4001, 4005, 'Plastic Pollution and Marine Ecosystems', 'Paragraph A: Each year, approximately eight million tonnes of plastic waste enter the world''s oceans, a figure equivalent to emptying a rubbish truck into the sea every minute. This pollution has been found in every marine environment, from surface waters to the deepest ocean trenches. A 2021 study detected microplastics in sediment samples taken from the Mariana Trench, nearly eleven kilometres below the surface.\n\nParagraph B: The impact on marine life is severe. Over seven hundred marine species are known to have ingested or become entangled in plastic debris. Sea turtles frequently mistake plastic bags for jellyfish, their primary food source. Seabirds such as albatrosses feed plastic fragments to their chicks, which can cause intestinal blockage and starvation. At the microscopic level, plankton have been observed consuming particles smaller than five millimetres, introducing toxins into the base of the food chain.\n\nParagraph C: Several strategies are being deployed to combat the crisis. Beach clean-up operations remove visible debris but address only a fraction of the problem, since an estimated seventy percent of ocean plastic sinks below the surface. More promising are upstream interventions: banning single-use plastics, improving waste collection infrastructure in coastal developing nations, and investing in biodegradable alternatives.\n\nParagraph D: The most ambitious initiative is The Ocean Cleanup project, which uses floating barriers to collect surface plastic in the Great Pacific Garbage Patch. Since its launch, the system has removed over two hundred and fifty tonnes of debris. However, scientists caution that removal alone is insufficient; without reducing plastic production, the ocean will continue to accumulate waste faster than it can be extracted.', 310, 'band_5_6', 'environment and marine science', 'Cambridge IELTS 20 style', '2026-05-26 12:00:00', 'https://images.unsplash.com/photo-1621451537084-482c73073a0f?q=80&w=1200', 'Paragraph A: Mỗi năm, khoảng tám triệu tấn rác nhựa đổ vào các đại dương trên thế giới, một con số tương đương với việc đổ một xe tải rác xuống biển mỗi phút.\n\nParagraph B: Tác động đến đời sống biển là nghiêm trọng. Hơn bảy trăm loài sinh vật biển được biết đã nuốt phải hoặc bị vướng vào mảnh vụn nhựa.\n\nParagraph C: Một số chiến lược đang được triển khai để chống lại cuộc khủng hoảng này.\n\nParagraph D: Sáng kiến tham vọng nhất là dự án The Ocean Cleanup, sử dụng các rào cản nổi để thu gom nhựa bề mặt.', '[{"word":"entangled","ipa":"/ɪnˈtæŋ.ɡəld/","vi":"bị mắc kẹt","en":"caught up in something twisted or complicated"},{"word":"microplastics","ipa":"/ˈmaɪ.krəʊˌplæs.tɪks/","vi":"vi nhựa","en":"tiny plastic particles less than 5mm in size"},{"word":"biodegradable","ipa":"/ˌbaɪ.əʊ.dɪˈɡreɪ.də.bəl/","vi":"phân hủy sinh học","en":"capable of being decomposed by bacteria or organisms"},{"word":"upstream","ipa":"/ˈʌp.striːm/","vi":"ngược dòng, phòng ngừa","en":"at an earlier stage in a process"},{"word":"accumulate","ipa":"/əˈkjuː.mjə.leɪt/","vi":"tích lũy","en":"to gather or build up over time"}]'),
-- Test 3 Passage 2
(4002, 4006, 'The Psychology of Happiness', 'Paragraph A: The scientific study of happiness — known formally as subjective well-being research — has expanded rapidly since the turn of the millennium. Psychologists now distinguish between two components of happiness: hedonic well-being, which refers to day-to-day pleasure and the absence of pain, and eudaimonic well-being, which involves a sense of purpose, personal growth and meaningful engagement with life.\n\nParagraph B: Large-scale surveys consistently show that beyond a certain income threshold, additional wealth contributes relatively little to reported happiness. A widely cited study by Princeton researchers in 2010 found that emotional well-being plateaued at an annual household income of approximately seventy-five thousand US dollars. More recent analysis using a larger dataset, published in 2023, suggested the threshold may be higher — around one hundred thousand dollars — but the fundamental principle remained: the relationship between money and happiness is logarithmic, not linear.\n\nParagraph C: Social relationships appear to be the strongest predictor of lasting happiness. The Harvard Study of Adult Development, which has tracked participants for over eighty years, concluded that the quality of close relationships at age fifty was a better predictor of physical health at eighty than cholesterol levels. Regular social interaction, acts of kindness, and community involvement all correlate strongly with higher life satisfaction scores.\n\nParagraph D: Interventions designed to increase happiness have shown mixed results. Gratitude journaling, where individuals record three positive events each day, produced measurable improvements in well-being lasting up to six months in controlled trials. Mindfulness meditation programmes have demonstrated reductions in anxiety and improvements in attention. However, critics note that many positive psychology interventions have small effect sizes and may not transfer well across different cultural contexts.', 310, 'band_6_7', 'psychology and health', 'Cambridge IELTS 20 style', '2026-05-26 12:00:00', NULL, NULL, '[{"word":"hedonic","ipa":"/hiːˈdɒn.ɪk/","vi":"thuộc khoái lạc","en":"relating to pleasure or enjoyment"},{"word":"eudaimonic","ipa":"/ˌjuː.daɪˈmɒn.ɪk/","vi":"thuộc hạnh phúc tự thân","en":"relating to happiness through purpose and meaning"},{"word":"plateau","ipa":"/ˈplæt.əʊ/","vi":"đạt mức ổn định","en":"to reach a level of little or no change"},{"word":"logarithmic","ipa":"/ˌlɒɡ.əˈrɪð.mɪk/","vi":"theo lôgarit","en":"increasing in proportion to the logarithm of a value"},{"word":"gratitude","ipa":"/ˈɡræt.ɪ.tjuːd/","vi":"lòng biết ơn","en":"the quality of being thankful"}]'),
-- Test 3 Passage 3
(4003, 4007, 'Saving Endangered Languages', 'Paragraph A: Of the approximately seven thousand languages spoken worldwide today, linguists estimate that nearly half are endangered — spoken by fewer than ten thousand people and declining. Every two weeks, on average, a language dies. When the last speaker of a language passes away, an entire system of knowledge, cultural identity and oral tradition is lost permanently.\n\nParagraph B: The causes of language decline are well documented. Globalisation and urbanisation encourage speakers to adopt dominant languages — primarily English, Mandarin and Spanish — for economic advancement. National education systems often teach exclusively in the official language, leaving minority languages without formal transmission mechanisms. In some cases, historical policies of forced assimilation deliberately suppressed indigenous languages.\n\nParagraph C: Preservation efforts take many forms. Documentation projects use audio and video recording to create permanent archives of endangered languages. In New Zealand, Māori language immersion schools, known as kōhanga reo, have reversed the decline of te reo Māori: the number of fluent speakers under twenty-five has increased by forty percent since the programme began in 1982. Digital technology has also contributed: mobile apps, social media content and online dictionaries make minority languages accessible to younger generations.\n\nParagraph D: However, preservation is not universally supported. Some scholars argue that language death is a natural evolutionary process and that resources spent on preservation could be better used elsewhere. Others question whether revitalisation efforts can truly restore a language to daily use or merely create an academic artefact. Advocates counter that linguistic diversity is as valuable as biological diversity and that each language represents a unique cognitive framework for understanding the world.', 310, 'band_6_7', 'linguistics and culture', 'Cambridge IELTS 20 style', '2026-05-26 12:00:00', NULL, NULL, '[{"word":"assimilation","ipa":"/əˌsɪm.ɪˈleɪ.ʃən/","vi":"sự đồng hóa","en":"the process of absorbing into a wider culture"},{"word":"immersion","ipa":"/ɪˈmɜː.ʃən/","vi":"nhúng chìm, đắm chìm","en":"deep involvement or being surrounded by something"},{"word":"revitalisation","ipa":"/ˌriː.vaɪ.təl.aɪˈzeɪ.ʃən/","vi":"sự hồi sinh","en":"the process of making something active or vigorous again"},{"word":"artefact","ipa":"/ˈɑː.tɪ.fækt/","vi":"hiện vật, sản phẩm","en":"an object made by a human, or something artificial"},{"word":"cognitive","ipa":"/ˈkɒɡ.nə.tɪv/","vi":"thuộc nhận thức","en":"relating to mental processes of understanding"}]'),
-- Test 4
(5001, 5005, 'Building Cities for Cyclists', 'Paragraph A: In the past two decades, cycling has been transformed from a recreational activity into a serious urban transport strategy. Cities such as Copenhagen, Amsterdam and Utrecht have demonstrated that investment in cycling infrastructure can reshape commuting patterns, reduce emissions and improve public health. Copenhagen''s cycling modal share now stands at forty-nine percent for commutes to work or education — up from thirty-six percent in 2012.\n\nParagraph B: The key to success, planners agree, is protected infrastructure. Painted cycle lanes on busy roads have been shown to be largely ineffective in encouraging new cyclists, whereas physically separated lanes — with kerbs, planters or parked cars as barriers — increase cycling rates by up to seventy-five percent. The Dutch concept of a fietspad, an entirely separate path network, has been replicated in several Scandinavian and German cities.\n\nParagraph C: Economic analysis supports the investment case. A cost-benefit study by the European Cyclists'' Federation found that every euro spent on cycling infrastructure returned between five and nine euros in health savings, reduced congestion and lower pollution. The average cost of building one kilometre of protected cycle lane is approximately one point five million euros, compared to over one hundred million euros for one kilometre of urban motorway.\n\nParagraph D: Barriers remain substantial. In car-dependent cities, reallocating road space to cyclists often faces political opposition from motorists and business owners who fear reduced access. Cold or wet climates are frequently cited as obstacles, though cities such as Oulu in Finland — where winter temperatures reach minus thirty degrees — achieve year-round cycling rates of twenty percent through heated cycle paths and dedicated snow clearance. Cultural attitudes may prove the most significant barrier: where driving is seen as a status symbol, shifting perceptions requires sustained public communication campaigns.', 320, 'band_5_6', 'transport and urban planning', 'Cambridge IELTS 20 style', '2026-05-26 12:00:00', 'https://images.unsplash.com/photo-1558618666-fcd25c85f82e?q=80&w=1200', 'Paragraph A: Trong hai thập kỷ qua, đạp xe đã được chuyển đổi từ một hoạt động giải trí thành một chiến lược giao thông đô thị nghiêm túc.\n\nParagraph B: Chìa khóa thành công là cơ sở hạ tầng được bảo vệ.\n\nParagraph C: Phân tích kinh tế ủng hộ trường hợp đầu tư.\n\nParagraph D: Các rào cản vẫn còn đáng kể.', '[{"word":"modal share","ipa":"/ˈməʊ.dəl ʃeər/","vi":"tỷ phần phương thức","en":"the percentage of trips using a particular transport mode"},{"word":"fietspad","ipa":"/fiːts.pɑːd/","vi":"đường xe đạp (tiếng Hà Lan)","en":"a separate cycling path, Dutch concept"},{"word":"congestion","ipa":"/kənˈdʒes.tʃən/","vi":"tắc nghẽn","en":"overcrowding and slow movement of traffic"},{"word":"reallocating","ipa":"/ˌriː.ˈæl.ə.keɪ.tɪŋ/","vi":"tái phân bổ","en":"distributing or assigning resources differently"},{"word":"sustained","ipa":"/səˈsteɪnd/","vi":"bền bỉ, liên tục","en":"continuing for an extended period"}]'),
-- Test 4 Passage 2
(5002, 5006, 'How the Brain Makes Decisions', 'Paragraph A: Every day, the average person makes approximately thirty-five thousand decisions, from trivial choices about what to eat to consequential judgments about career and relationships. Neuroscientists have identified two distinct neural systems involved in decision-making: System 1, which operates automatically and quickly with little effort, and System 2, which allocates attention to effortful mental activities and complex computations.\n\nParagraph B: The prefrontal cortex plays a central role in System 2 thinking — deliberate reasoning, planning and impulse control. Damage to this region, as in the famous case of Phineas Gage in 1848, can result in impaired judgment while leaving other cognitive abilities intact. Brain imaging studies show that the prefrontal cortex is among the last brain regions to fully mature, typically around age twenty-five, which may explain the higher risk-taking behaviour observed in adolescents.\n\nParagraph C: Emotions, far from being obstacles to good decision-making, appear to play an essential role. The somatic marker hypothesis, proposed by neuroscientist Antonio Damasio, suggests that emotional responses to previous experiences create bodily signals that guide future choices. Patients with damage to the ventromedial prefrontal cortex — the region that integrates emotional and rational processing — consistently make poor real-life decisions despite performing normally on standard intelligence tests.\n\nParagraph D: Recent research has explored how digital environments affect decision quality. Information overload — having too many options — can trigger "choice paralysis," where individuals delay or avoid making decisions altogether. A 2023 study at Stanford found that participants presented with twenty-four jam varieties were ten times less likely to make a purchase than those offered only six. This has implications for website design, retirement planning interfaces and healthcare options, where simplifying choices can lead to better outcomes.', 310, 'band_6_7', 'neuroscience and psychology', 'Cambridge IELTS 20 style', '2026-05-26 12:00:00', NULL, NULL, '[{"word":"prefrontal cortex","ipa":"/ˌpriːˈfrʌn.təl ˈkɔː.teks/","vi":"vỏ não trán trước","en":"the front part of the brain involved in planning and decision-making"},{"word":"somatic","ipa":"/səʊˈmæt.ɪk/","vi":"thuộc cơ thể","en":"relating to the body rather than the mind"},{"word":"hypothesis","ipa":"/haɪˈpɒθ.ə.sɪs/","vi":"giả thuyết","en":"a proposed explanation for a phenomenon"},{"word":"ventromedial","ipa":"/ˌven.trəʊˈmiː.di.əl/","vi":"bụng giữa","en":"relating to the front-middle area of a brain structure"},{"word":"paralysis","ipa":"/pəˈræl.ə.sɪs/","vi":"tê liệt, bế tắc","en":"inability to act or decide"}]'),
-- Test 4 Passage 3
(5003, 5007, 'The Ethics of Genetic Engineering', 'Paragraph A: The development of CRISPR-Cas9 gene editing technology in 2012 transformed genetic engineering from a slow, expensive laboratory procedure into a precise, affordable tool accessible to researchers worldwide. CRISPR allows scientists to cut, delete or insert specific DNA sequences with unprecedented accuracy, opening possibilities for curing genetic diseases, improving crop resilience and even modifying human embryos.\n\nParagraph B: Medical applications have advanced rapidly. Clinical trials are underway for CRISPR-based treatments for sickle cell disease, beta-thalassaemia and certain cancers. In December 2023, the UK and US approved Casgevy, the first CRISPR therapy, for sickle cell disease. The treatment involves extracting a patient''s stem cells, editing them to produce functional haemoglobin, and reinfusing them — a process that has eliminated painful crises in ninety-seven percent of trial participants.\n\nParagraph C: Agricultural applications are equally transformative. Gene-edited crops can be designed to resist drought, tolerate salt and provide enhanced nutritional content. A CRISPR-modified tomato with elevated levels of GABA — a compound linked to lower blood pressure — was approved for sale in Japan in 2021. Unlike earlier genetically modified organisms, gene-edited crops do not contain foreign DNA and are therefore regulated differently in many countries.\n\nParagraph D: The most contentious area is germline editing — modifications to embryos that would be inherited by future generations. In 2018, Chinese scientist He Jiankui announced the birth of twin girls whose genes had been edited to resist HIV infection. The experiment was widely condemned by the scientific community as premature, poorly designed and ethically unjustifiable. Most countries now prohibit human germline editing for reproductive purposes, though the regulatory landscape remains fragmented and enforcement mechanisms are weak.', 310, 'band_6_7', 'science and ethics', 'Cambridge IELTS 20 style', '2026-05-26 12:00:00', NULL, NULL, '[{"word":"CRISPR","ipa":"/ˈkrɪs.pər/","vi":"CRISPR (công cụ chỉnh sửa gen)","en":"a technology for editing genes with high precision"},{"word":"germline","ipa":"/ˈdʒɜːm.laɪn/","vi":"dòng mầm","en":"cells whose genetic information is passed to the next generation"},{"word":"haemoglobin","ipa":"/ˌhiː.məˈɡləʊ.bɪn/","vi":"huyết sắc tố","en":"the protein in red blood cells that carries oxygen"},{"word":"contentious","ipa":"/kənˈten.ʃəs/","vi":"gây tranh cãi","en":"causing or likely to cause disagreement"},{"word":"premature","ipa":"/ˈprem.ə.tʃʊər/","vi":"sớm, chưa chín muồi","en":"occurring or done before the proper time"}]');


--
-- Reading Questions for Tests 2-4
--
INSERT INTO `tb_reading_questions` (`question_id`, `passage_id`, `question_type`, `question_number`, `question_text`, `correct_answer`, `options`, `explanation`, `band_target`, `paragraph_reference`, `created_at`) VALUES
-- Test 2 Passage 1 (Urban Farming)
(3001, 3001, 'true_false_not_given', 1, 'Urban farming supplies food to eight hundred million people.', 'True', '["True","False","Not Given"]', 'FAO report estimate stated in Paragraph A.', 6.0, 'A', '2026-05-26 12:00:00'),
(3002, 3001, 'sentence_completion', 2, 'Hydroponic systems use ____% less water than conventional farming.', '90', NULL, 'Paragraph B states ninety percent less.', 6.0, 'B', '2026-05-26 12:00:00'),
(3003, 3001, 'multiple_choice', 3, 'Community gardeners consumed more:', 'B', '["A. protein","B. fruit and vegetables","C. grains","D. dairy"]', 'Forty percent more fruit and vegetables per Paragraph C.', 6.0, 'C', '2026-05-26 12:00:00'),
(3004, 3001, 'sentence_completion', 4, 'Contaminated ____ in industrial areas poses health risks.', 'soil', NULL, 'Paragraph D mentions contaminated soil.', 6.0, 'D', '2026-05-26 12:00:00'),
(3005, 3001, 'true_false_not_given', 5, 'Vertical farming always reduces environmental impact.', 'False', '["True","False","Not Given"]', 'Energy costs can offset gains per Paragraph D.', 6.5, 'D', '2026-05-26 12:00:00'),
-- Test 2 Passage 2 (Digital Literacy)
(3006, 3002, 'sentence_completion', 6, 'Only ____% of fifteen-year-olds could distinguish fact from opinion.', '52', NULL, 'Paragraph B OECD survey finding.', 6.5, 'B', '2026-05-26 12:00:00'),
(3007, 3002, 'multiple_choice', 7, 'Which country first integrated media evaluation into curriculum?', 'C', '["A. UK","B. Germany","C. Finland","D. Sweden"]', 'Finland since 2016 per Paragraph B.', 6.5, 'B', '2026-05-26 12:00:00'),
(3008, 3002, 'sentence_completion', 8, 'Advanced literacy includes understanding ____ fundamentals.', 'coding', NULL, 'Paragraph C lists coding as advanced skill.', 6.5, 'C', '2026-05-26 12:00:00'),
(3009, 3002, 'true_false_not_given', 9, 'Equipping classrooms costs about twelve thousand pounds each.', 'True', '["True","False","Not Given"]', 'UK estimate in Paragraph D.', 6.5, 'D', '2026-05-26 12:00:00'),
(3010, 3002, 'multiple_choice', 10, 'The passage overall argues that digital literacy is:', 'A', '["A. essential despite costs","B. unnecessary for young children","C. harmful to learning","D. only for advanced students"]', 'Advocates say illiteracy is more costly long-term.', 7.0, 'D', '2026-05-26 12:00:00'),
-- Test 2 Passage 3 (Behavioural Economics)
(3011, 3003, 'sentence_completion', 11, 'People use mental shortcuts called ____ for quick judgments.', 'heuristics', NULL, 'Paragraph A defines heuristics.', 7.0, 'A', '2026-05-26 12:00:00'),
(3012, 3003, 'true_false_not_given', 12, 'Opt-out pension enrollment raised participation to 93%.', 'True', '["True","False","Not Given"]', 'Paragraph B UK study figure.', 7.0, 'B', '2026-05-26 12:00:00'),
(3013, 3003, 'sentence_completion', 13, 'Personalised tax letters increased payments by ____%.', '15', NULL, 'Paragraph C Guatemala study.', 7.0, 'C', '2026-05-26 12:00:00'),
(3014, 3003, 'multiple_choice', 14, 'Critics of nudging are primarily concerned about:', 'B', '["A. cost","B. manipulation and autonomy","C. complexity","D. scientific validity"]', 'Paragraph D raises manipulation and paternalism.', 7.0, 'D', '2026-05-26 12:00:00'),
(3015, 3003, 'true_false_not_given', 15, 'All nudge interventions are disclosed to participants.', 'False', '["True","False","Not Given"]', 'Effective because people are unaware per Paragraph D.', 7.0, 'D', '2026-05-26 12:00:00'),
-- Test 3 Passage 1 (Plastic)
(4001, 4001, 'sentence_completion', 1, '____ million tonnes of plastic enter oceans annually.', '8', NULL, 'Paragraph A gives eight million.', 6.0, 'A', '2026-05-26 12:00:00'),
(4002, 4001, 'true_false_not_given', 2, 'Microplastics have been found in the deepest ocean trenches.', 'True', '["True","False","Not Given"]', 'Mariana Trench detection in Paragraph A.', 6.0, 'A', '2026-05-26 12:00:00'),
(4003, 4001, 'sentence_completion', 3, 'Over ____ marine species have ingested plastic.', '700', NULL, 'Paragraph B gives seven hundred.', 6.0, 'B', '2026-05-26 12:00:00'),
(4004, 4001, 'multiple_choice', 4, 'What percentage of ocean plastic sinks below the surface?', 'C', '["A. 30%","B. 50%","C. 70%","D. 90%"]', 'Seventy percent sinks per Paragraph C.', 6.5, 'C', '2026-05-26 12:00:00'),
(4005, 4001, 'sentence_completion', 5, 'The Ocean Cleanup has removed over ____ tonnes.', '250', NULL, 'Over two hundred and fifty tonnes per Paragraph D.', 6.0, 'D', '2026-05-26 12:00:00'),
-- Test 3 Passage 2 (Happiness)
(4006, 4002, 'multiple_choice', 6, 'Eudaimonic well-being involves:', 'B', '["A. daily pleasure","B. purpose and personal growth","C. financial security","D. physical comfort"]', 'Paragraph A defines eudaimonic well-being.', 6.5, 'A', '2026-05-26 12:00:00'),
(4007, 4002, 'sentence_completion', 7, 'Emotional well-being plateaus at around $____ income.', '75,000', NULL, 'Paragraph B Princeton study.', 6.5, 'B', '2026-05-26 12:00:00'),
(4008, 4002, 'true_false_not_given', 8, 'The Harvard study has tracked participants for over 80 years.', 'True', '["True","False","Not Given"]', 'Stated in Paragraph C.', 6.5, 'C', '2026-05-26 12:00:00'),
(4009, 4002, 'sentence_completion', 9, 'Gratitude journaling improvements lasted up to ____ months.', '6', NULL, 'Up to six months per Paragraph D.', 6.5, 'D', '2026-05-26 12:00:00'),
(4010, 4002, 'true_false_not_given', 10, 'Positive psychology interventions work equally across all cultures.', 'False', '["True","False","Not Given"]', 'May not transfer across cultural contexts.', 7.0, 'D', '2026-05-26 12:00:00'),
-- Test 3 Passage 3 (Languages)
(4011, 4003, 'sentence_completion', 11, 'A language dies approximately every ____ weeks.', 'two', NULL, 'Every two weeks per Paragraph A.', 6.5, 'A', '2026-05-26 12:00:00'),
(4012, 4003, 'multiple_choice', 12, 'Historical language decline was sometimes caused by:', 'D', '["A. natural disasters","B. economic reform","C. scientific progress","D. forced assimilation policies"]', 'Paragraph B mentions forced assimilation.', 7.0, 'B', '2026-05-26 12:00:00'),
(4013, 4003, 'sentence_completion', 13, 'Māori immersion schools increased fluent speakers by ____%.', '40', NULL, 'Forty percent increase per Paragraph C.', 7.0, 'C', '2026-05-26 12:00:00'),
(4014, 4003, 'true_false_not_given', 14, 'All scholars support language preservation efforts.', 'False', '["True","False","Not Given"]', 'Some argue it is a natural process per Paragraph D.', 7.0, 'D', '2026-05-26 12:00:00'),
(4015, 4003, 'multiple_choice', 15, 'Advocates of preservation compare linguistic diversity to:', 'A', '["A. biological diversity","B. economic growth","C. technological progress","D. artistic heritage"]', 'Paragraph D comparison.', 7.0, 'D', '2026-05-26 12:00:00'),
-- Test 4 Passage 1 (Cycling)
(5001, 5001, 'sentence_completion', 1, 'Copenhagen cycling modal share is now ____%.', '49', NULL, 'Forty-nine percent per Paragraph A.', 6.0, 'A', '2026-05-26 12:00:00'),
(5002, 5001, 'true_false_not_given', 2, 'Painted cycle lanes effectively encourage new cyclists.', 'False', '["True","False","Not Given"]', 'Largely ineffective per Paragraph B.', 6.0, 'B', '2026-05-26 12:00:00'),
(5003, 5001, 'sentence_completion', 3, 'Every euro spent returns between ____ and nine euros.', '5', NULL, 'Five to nine euros per Paragraph C.', 6.5, 'C', '2026-05-26 12:00:00'),
(5004, 5001, 'sentence_completion', 4, 'One km of cycle lane costs about ____ million euros.', '1.5', NULL, 'Approximately 1.5 million euros.', 6.0, 'C', '2026-05-26 12:00:00'),
(5005, 5001, 'multiple_choice', 5, 'The most significant barrier to cycling adoption is:', 'D', '["A. weather","B. infrastructure cost","C. safety concerns","D. cultural attitudes"]', 'Cultural attitudes may be most significant per Paragraph D.', 6.5, 'D', '2026-05-26 12:00:00'),
-- Test 4 Passage 2 (Decision Making)
(5006, 5002, 'sentence_completion', 6, 'People make approximately ____ decisions daily.', '35,000', NULL, 'Thirty-five thousand per Paragraph A.', 6.5, 'A', '2026-05-26 12:00:00'),
(5007, 5002, 'true_false_not_given', 7, 'The prefrontal cortex fully matures around age 25.', 'True', '["True","False","Not Given"]', 'Typically around age twenty-five per Paragraph B.', 6.5, 'B', '2026-05-26 12:00:00'),
(5008, 5002, 'sentence_completion', 8, 'The somatic marker hypothesis was proposed by ____.', 'Antonio Damasio', NULL, 'Neuroscientist Antonio Damasio per Paragraph C.', 7.0, 'C', '2026-05-26 12:00:00'),
(5009, 5002, 'multiple_choice', 9, 'Choice paralysis occurs when people face:', 'A', '["A. too many options","B. too few options","C. time pressure","D. emotional stress"]', 'Information overload triggers paralysis per Paragraph D.', 7.0, 'D', '2026-05-26 12:00:00'),
(5010, 5002, 'sentence_completion', 10, 'Participants with 24 jam varieties were ____ times less likely to buy.', '10', NULL, 'Ten times less likely per Paragraph D.', 7.0, 'D', '2026-05-26 12:00:00'),
-- Test 4 Passage 3 (Genetic Engineering)
(5011, 5003, 'sentence_completion', 11, 'CRISPR-Cas9 was developed in ____.', '2012', NULL, 'Developed in 2012 per Paragraph A.', 7.0, 'A', '2026-05-26 12:00:00'),
(5012, 5003, 'true_false_not_given', 12, 'Casgevy eliminated painful crises in 97% of participants.', 'True', '["True","False","Not Given"]', 'Ninety-seven percent per Paragraph B.', 7.0, 'B', '2026-05-26 12:00:00'),
(5013, 5003, 'sentence_completion', 13, 'A CRISPR tomato with elevated ____ was approved in Japan.', 'GABA', NULL, 'Elevated GABA levels per Paragraph C.', 7.0, 'C', '2026-05-26 12:00:00'),
(5014, 5003, 'multiple_choice', 14, 'He Jiankui''s experiment was condemned because it was:', 'C', '["A. too expensive","B. technically impossible","C. premature and ethically unjustifiable","D. government-sponsored"]', 'Paragraph D describes it as premature and unethical.', 7.0, 'D', '2026-05-26 12:00:00'),
(5015, 5003, 'true_false_not_given', 15, 'All countries have strong enforcement against germline editing.', 'False', '["True","False","Not Given"]', 'Enforcement mechanisms are weak per Paragraph D.', 7.0, 'D', '2026-05-26 12:00:00');


--
-- Writing Prompts for Tests 2-4
--
INSERT INTO `tb_writing_prompts` (`prompt_id`, `task_type`, `prompt_text`, `prompt_image_url`, `chart_type`, `difficulty_level`, `category`, `band_target`, `sample_answer`, `notes_for_teacher`, `is_active`, `created_at`, `updated_at`, `status`, `created_by`, `reviewed_by`, `reviewed_at`, `reviewer_note`, `is_deleted`) VALUES
-- Test 2
(3001, 'task1', 'The bar chart below shows the average daily water consumption per person in five different countries in 2020. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', NULL, 'bar', 3, 'environment', 7, 'The bar chart compares daily per-capita water usage across five nations in 2020. Overall, the United States had the highest consumption while India had the lowest.\n\nThe US led with approximately 375 litres per person daily, more than double India''s figure of 135 litres. Australia ranked second at 340 litres, followed by Japan at 280 litres. The UK consumed roughly 150 litres per person, only slightly above India.\n\nNotably, there was a significant gap between the top two countries and the remaining three. The US and Australia — both large, developed nations with warm climates — used considerably more water than the UK, Japan or India, suggesting that climate and economic factors influence consumption patterns.', 'Check data comparison language and overview quality.', 1, '2026-05-26 12:00:00', '2026-05-26 12:00:00', 'published', NULL, NULL, NULL, NULL, 0),
(3002, 'task2', 'International tourism has brought enormous benefits to many places. At the same time, there are concerns about its impact on local communities and the environment. Discuss both views and give your own opinion.', NULL, NULL, 3, 'tourism and environment', 7, 'International tourism generates significant economic benefits but also creates environmental and social challenges that require careful management.\n\nOn the positive side, tourism creates employment in hospitality, transport and retail sectors. Countries such as Thailand derive approximately twenty percent of GDP from tourism-related activities. Cultural exchange enriches both visitors and hosts, promoting mutual understanding and tolerance.\n\nHowever, mass tourism can overwhelm infrastructure, drive up housing costs for locals, and damage fragile ecosystems. Venice receives thirty million visitors annually — sixty times its resident population — leading to chronic overcrowding, water pollution and the displacement of local businesses by souvenir shops.\n\nIn my view, the benefits of tourism outweigh the drawbacks provided that governments implement sustainable tourism policies: visitor caps at sensitive sites, revenue-sharing with local communities, and environmental impact assessments for new developments. Tourism should enrich destinations, not exploit them.', 'Evaluate balanced discussion and strength of opinion.', 1, '2026-05-26 12:00:00', '2026-05-26 12:00:00', 'published', NULL, NULL, NULL, NULL, 0),
-- Test 3
(4001, 'task1', 'The line graph below shows the number of students enrolled at three different universities between 2010 and 2023. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', NULL, 'line', 3, 'education', 7, 'The line graph illustrates enrollment trends at three universities over a thirteen-year period. Overall, all three institutions experienced growth, though at different rates.\n\nUniversity A began with approximately 15,000 students in 2010 and grew steadily to reach 28,000 by 2023, representing the most consistent upward trend. University B started at a similar level but experienced a sharp increase between 2015 and 2018, rising from 16,000 to 25,000 before plateauing.\n\nUniversity C had the lowest initial enrollment at around 8,000 but showed the most dramatic proportional growth, more than tripling to 26,000 by 2023. Between 2018 and 2020, University C overtook University B.\n\nIn summary, while all three universities grew, the patterns differed significantly, with University C showing the most remarkable transformation.', 'Check trend vocabulary and comparison structures.', 1, '2026-05-26 12:00:00', '2026-05-26 12:00:00', 'published', NULL, NULL, NULL, NULL, 0),
(4002, 'task2', 'The widespread use of technology means that personal data is increasingly at risk. What problems does this cause, and what are some possible solutions?', NULL, NULL, 3, 'technology and society', 7, 'The digital age has created unprecedented risks to personal privacy, but well-designed policies and technologies can mitigate the most serious threats.\n\nThe primary problem is unauthorised data collection. Many apps and websites harvest user data without meaningful consent, building detailed profiles that can be sold to advertisers or exploited by criminals. Identity theft alone cost consumers over sixteen billion dollars globally in 2022. Additionally, government surveillance programmes — revealed by whistleblowers such as Edward Snowden — have shown that even democratic states may monitor citizens beyond legal boundaries.\n\nSeveral solutions deserve consideration. First, stronger data protection legislation such as the EU''s General Data Protection Regulation should be adopted globally, giving individuals the right to access, correct and delete their personal data. Second, technology companies should implement privacy-by-design principles, encrypting data by default and minimising collection to what is strictly necessary. Third, digital literacy education can empower users to recognise phishing attempts, use strong passwords and understand privacy settings.\n\nIn conclusion, protecting personal data requires coordinated action from lawmakers, technology companies and informed citizens.', 'Check problem-solution structure and feasibility.', 1, '2026-05-26 12:00:00', '2026-05-26 12:00:00', 'published', NULL, NULL, NULL, NULL, 0),
-- Test 4
(5001, 'task1', 'The two pie charts below compare the sources of electricity generation in Country X in 2000 and 2023. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.', NULL, 'pie', 3, 'energy', 7, 'The pie charts illustrate how Country X''s electricity sources changed over a twenty-three-year period. Overall, there was a significant shift away from fossil fuels toward renewable energy.\n\nIn 2000, coal dominated at 45% of total generation, followed by natural gas at 25% and nuclear at 20%. Renewables — comprising hydro, wind and solar — accounted for just 10%.\n\nBy 2023, the picture had changed dramatically. Coal''s share fell to 18%, while natural gas remained relatively stable at 22%. Nuclear declined slightly to 15%. Renewables experienced the most significant growth, rising to 40% of total generation, with wind and solar together contributing 28%.\n\nThe most notable changes were the halving of coal''s share and the quadrupling of renewable energy, reflecting global trends toward decarbonisation.', 'Check comparison language and overview sentence.', 1, '2026-05-26 12:00:00', '2026-05-26 12:00:00', 'published', NULL, NULL, NULL, NULL, 0),
(5002, 'task2', 'In many countries, people are moving from rural areas to cities. Why is this happening, and what challenges does it create?', NULL, NULL, 3, 'urbanisation', 7, 'Rural-to-urban migration is a global phenomenon driven by economic opportunity, but it creates significant infrastructure and social challenges that cities must address.\n\nThe primary driver is employment. Cities offer a wider range of jobs with higher wages than rural areas. In China alone, over three hundred million people have moved to cities since 1980, attracted by factory and service-sector work. Education is another factor: universities and training centres are concentrated in urban areas, drawing young people seeking qualifications. Additionally, cities provide better access to healthcare, entertainment and social networks.\n\nHowever, rapid urbanisation strains infrastructure. Housing shortages drive up costs, forcing low-income migrants into informal settlements with poor sanitation. Traffic congestion worsens air quality — the World Health Organization estimates that urban air pollution causes 4.2 million premature deaths annually. Public services such as schools and hospitals become overcrowded, reducing quality for all residents.\n\nGovernments can respond by investing in affordable housing, expanding public transport networks, and developing satellite cities that distribute population pressure. Ultimately, making rural areas more attractive through improved infrastructure and connectivity would reduce the intensity of migration.', 'Evaluate cause-effect structure and solution feasibility.', 1, '2026-05-26 12:00:00', '2026-05-26 12:00:00', 'published', NULL, NULL, NULL, NULL, 0);


--
-- Additional Speaking Topics for Tests 2-4
--
INSERT INTO `tb_speaking_topics` (`topic_id`, `part`, `topic_title`, `description`, `difficulty_level`, `band_target`, `is_active`, `created_at`, `updated_at`, `status`, `created_by`, `reviewed_by`, `reviewed_at`, `reviewer_note`, `is_deleted`) VALUES
-- Test 2 Speaking Topics
(3001, '1', 'Your neighbourhood', 'Describe where you live, what you like and dislike about it, and how it has changed.', 2, 6, 1, '2026-05-26 12:00:00', '2026-05-26 12:00:00', 'published', NULL, NULL, NULL, NULL, 0),
(3002, '2', 'A book that influenced you', 'Describe a book that had a significant impact on your thinking or life. You should say what the book was about, when you read it, why it was important to you, and how it changed your perspective.', 3, 7, 1, '2026-05-26 12:00:00', '2026-05-26 12:00:00', 'published', NULL, NULL, NULL, NULL, 0),
(3003, '3', 'Reading habits in the digital age', 'Discuss whether people read more or less than in the past, the advantages and disadvantages of e-books versus print books, and whether schools should encourage reading more.', 4, 7, 1, '2026-05-26 12:00:00', '2026-05-26 12:00:00', 'published', NULL, NULL, NULL, NULL, 0),
-- Test 3 Speaking Topics
(4001, '1', 'Work and daily routine', 'Talk about your work or studies, your daily schedule, and how you manage your time effectively.', 2, 6, 1, '2026-05-26 12:00:00', '2026-05-26 12:00:00', 'published', NULL, NULL, NULL, NULL, 0),
(4002, '2', 'A practical skill you learned', 'Describe a practical skill you learned outside of school or university. You should say what the skill was, how you learned it, why you needed it, and how useful it has been.', 3, 7, 1, '2026-05-26 12:00:00', '2026-05-26 12:00:00', 'published', NULL, NULL, NULL, NULL, 0),
(4003, '3', 'Lifelong learning and education', 'Discuss whether learning should continue throughout life, the role of online courses, and how governments can encourage adults to keep learning.', 4, 7, 1, '2026-05-26 12:00:00', '2026-05-26 12:00:00', 'published', NULL, NULL, NULL, NULL, 0),
-- Test 4 Speaking Topics
(5001, '1', 'Hobbies and free time activities', 'Talk about what you enjoy doing in your spare time, whether your hobbies have changed over time, and why free time is important.', 2, 6, 1, '2026-05-26 12:00:00', '2026-05-26 12:00:00', 'published', NULL, NULL, NULL, NULL, 0),
(5002, '2', 'A personal achievement', 'Describe something you achieved that you are particularly proud of. You should say what you did, when it happened, what difficulties you faced, and why it mattered to you.', 3, 7, 1, '2026-05-26 12:00:00', '2026-05-26 12:00:00', 'published', NULL, NULL, NULL, NULL, 0),
(5003, '3', 'Success and motivation', 'Discuss what factors contribute to success, whether success depends more on talent or hard work, and how society can better motivate young people.', 4, 7, 1, '2026-05-26 12:00:00', '2026-05-26 12:00:00', 'published', NULL, NULL, NULL, NULL, 0);


--
-- Speaking Topic Parts (Follow-up Questions)
--
INSERT INTO `tb_speaking_topic_parts` (`part_id`, `topic_id`, `part_number`, `content_text`, `time_limit_seconds`, `sequence_order`, `is_follow_up`) VALUES
-- Test 2 Part 1
(3001, 3001, 1, 'What do you like most about your neighbourhood?', 30, 1, 0),
(3002, 3001, 1, 'Has your area changed much in recent years?', 30, 2, 1),
(3003, 3001, 1, 'Do you think you will continue living there?', 30, 3, 1),
-- Test 2 Part 2
(3004, 3002, 2, 'Describe a book that influenced you.\n- What was the book about?\n- When did you read it?\n- Why was it important?\n- How did it change your perspective?', 120, 1, 0),
(3005, 3002, 2, 'Do you still read books regularly?', 30, 2, 1),
-- Test 2 Part 3
(3006, 3003, 3, 'Do you think people read more or less than in the past?', 60, 1, 0),
(3007, 3003, 3, 'What are the advantages of e-books over traditional books?', 60, 2, 1),
(3008, 3003, 3, 'Should schools do more to encourage reading among children?', 60, 3, 1),
-- Test 3 Part 1
(4001, 4001, 1, 'Do you work or are you a student?', 30, 1, 0),
(4002, 4001, 1, 'What does a typical day look like for you?', 30, 2, 1),
(4003, 4001, 1, 'How do you manage your time effectively?', 30, 3, 1),
-- Test 3 Part 2
(4004, 4002, 2, 'Describe a practical skill you learned.\n- What was the skill?\n- How did you learn it?\n- Why did you need it?\n- How useful has it been?', 120, 1, 0),
(4005, 4002, 2, 'Would you recommend others learn this skill?', 30, 2, 1),
-- Test 3 Part 3
(4006, 4003, 3, 'Is it important for adults to continue learning throughout their lives?', 60, 1, 0),
(4007, 4003, 3, 'How have online courses changed education?', 60, 2, 1),
(4008, 4003, 3, 'What can governments do to encourage lifelong learning?', 60, 3, 1),
-- Test 4 Part 1
(5001, 5001, 1, 'What do you enjoy doing in your free time?', 30, 1, 0),
(5002, 5001, 1, 'Have your hobbies changed since you were younger?', 30, 2, 1),
(5003, 5001, 1, 'Is it important for people to have hobbies?', 30, 3, 1),
-- Test 4 Part 2
(5004, 5002, 2, 'Describe a personal achievement you are proud of.\n- What did you achieve?\n- When did it happen?\n- What difficulties did you face?\n- Why does it matter to you?', 120, 1, 0),
(5005, 5002, 2, 'Do you think you will try to achieve something similar again?', 30, 2, 1),
-- Test 4 Part 3
(5006, 5003, 3, 'What factors contribute most to success in life?', 60, 1, 0),
(5007, 5003, 3, 'Is success more about talent or hard work?', 60, 2, 1),
(5008, 5003, 3, 'How can society better motivate young people to achieve their goals?', 60, 3, 1);


--
-- Additional IELTS Vocabulary (50 more essential words)
--
INSERT INTO `tb_vocabulary` (`vocab_id`, `word`, `phonetic`, `part_of_speech`, `ielts_topic`, `usage_frequency`, `meaning`, `example`, `difficulty`, `synonyms`, `antonyms`, `audio_url`) VALUES
(43, 'phenomenon', '/fɪˈnɒmɪnən/', 'noun', 'science', 3, 'Hiện tượng', 'Climate change is a global phenomenon affecting every continent.', 7, 'occurrence,event', 'normality', NULL),
(44, 'deteriorate', '/dɪˈtɪəriəreɪt/', 'verb', 'health', 2, 'Xấu đi, xuống cấp', 'Air quality continues to deteriorate in heavily industrialised regions.', 8, 'worsen,decline', 'improve,recover', NULL),
(45, 'scrutinise', '/ˈskruːtɪnaɪz/', 'verb', 'society', 2, 'Xem xét kỹ lưỡng', 'Governments should scrutinise the impact of new policies before implementation.', 8, 'examine,inspect', 'overlook,ignore', NULL),
(46, 'plausible', '/ˈplɔːzɪbl/', 'adjective', 'science', 2, 'Hợp lý, có vẻ đúng', 'The researcher presented a plausible explanation for the anomaly.', 8, 'credible,reasonable', 'implausible,unlikely', NULL),
(47, 'incentive', '/ɪnˈsentɪv/', 'noun', 'economics', 3, 'Động lực, ưu đãi', 'Tax incentives can encourage businesses to invest in renewable energy.', 6, 'motivation,stimulus', 'deterrent,disincentive', NULL),
(48, 'alleviate', '/əˈliːvieɪt/', 'verb', 'health', 2, 'Giảm nhẹ, xoa dịu', 'Medication can alleviate the symptoms but not cure the underlying condition.', 7, 'ease,relieve', 'aggravate,intensify', NULL),
(49, 'paradigm', '/ˈpærədaɪm/', 'noun', 'science', 2, 'Mô hình, khuôn mẫu', 'The digital revolution has created a new paradigm in communication.', 8, 'model,framework', 'anomaly', NULL),
(50, 'affluent', '/ˈæfluənt/', 'adjective', 'economics', 2, 'Giàu có, sung túc', 'Affluent nations consume a disproportionate share of global resources.', 7, 'wealthy,prosperous', 'impoverished,destitute', NULL),
(51, 'inevitable', '/ɪnˈevɪtəbl/', 'adjective', 'society', 3, 'Không thể tránh khỏi', 'Some degree of technological disruption is inevitable in every industry.', 6, 'unavoidable,certain', 'avoidable,preventable', NULL),
(52, 'pragmatic', '/præɡˈmætɪk/', 'adjective', 'society', 2, 'Thực dụng, thực tế', 'A pragmatic approach to climate policy balances economic and environmental goals.', 8, 'practical,realistic', 'idealistic,impractical', NULL),
(53, 'ambiguous', '/æmˈbɪɡjuəs/', 'adjective', 'general', 2, 'Mơ hồ, không rõ ràng', 'The instructions were ambiguous, causing confusion among participants.', 7, 'unclear,vague', 'clear,explicit', NULL),
(54, 'supplement', '/ˈsʌplɪment/', 'verb', 'health', 3, 'Bổ sung, thêm vào', 'Many people supplement their diet with vitamins and minerals.', 6, 'add,complement', 'reduce,deplete', NULL),
(55, 'deterioration', '/dɪˌtɪəriəˈreɪʃn/', 'noun', 'environment', 2, 'Sự suy thoái, xuống cấp', 'The deterioration of coral reefs threatens marine biodiversity.', 8, 'decline,degradation', 'improvement,recovery', NULL),
(56, 'comprehensive', '/ˌkɒmprɪˈhensɪv/', 'adjective', 'education', 3, 'Toàn diện, đầy đủ', 'The report provides a comprehensive analysis of current trends.', 6, 'thorough,complete', 'partial,limited', NULL),
(57, 'eradicate', '/ɪˈrædɪkeɪt/', 'verb', 'health', 2, 'Xóa bỏ, tiêu diệt hoàn toàn', 'Vaccination campaigns aim to eradicate preventable diseases.', 8, 'eliminate,abolish', 'establish,introduce', NULL),
(58, 'segregation', '/ˌseɡrɪˈɡeɪʃn/', 'noun', 'society', 2, 'Sự phân biệt, tách biệt', 'Residential segregation by income level remains a persistent urban issue.', 8, 'separation,division', 'integration,unity', NULL),
(59, 'accumulate', '/əˈkjuːmjuleɪt/', 'verb', 'general', 3, 'Tích lũy, chất chồng', 'Plastic waste continues to accumulate in the world''s oceans.', 6, 'gather,amass', 'disperse,distribute', NULL),
(60, 'empathy', '/ˈempəθi/', 'noun', 'society', 3, 'Sự đồng cảm', 'Cultural exchange programmes develop empathy between communities.', 7, 'understanding,compassion', 'indifference,apathy', NULL),
(61, 'innovation', '/ˌɪnəˈveɪʃn/', 'noun', 'technology', 3, 'Sự đổi mới, sáng tạo', 'Technological innovation drives economic growth in developed nations.', 6, 'invention,creativity', 'tradition,convention', NULL),
(62, 'sustainability', '/səˌsteɪnəˈbɪləti/', 'noun', 'environment', 3, 'Tính bền vững', 'Sustainability should be a core principle in urban planning.', 7, 'durability,viability', 'unsustainability', NULL),
(63, 'indigenous', '/ɪnˈdɪdʒɪnəs/', 'adjective', 'culture', 2, 'Bản địa, nguyên thủy', 'Indigenous communities possess invaluable traditional ecological knowledge.', 7, 'native,aboriginal', 'foreign,imported', NULL),
(64, 'alleviation', '/əˌliːviˈeɪʃn/', 'noun', 'society', 2, 'Sự giảm nhẹ, giảm bớt', 'Poverty alleviation remains a key priority for international development.', 8, 'relief,reduction', 'aggravation,worsening', NULL),
(65, 'hierarchy', '/ˈhaɪərɑːki/', 'noun', 'society', 2, 'Hệ thống thứ bậc', 'Traditional workplace hierarchies are being replaced by flat structures.', 7, 'ranking,order', 'equality,flatness', NULL),
(66, 'consensus', '/kənˈsensəs/', 'noun', 'society', 3, 'Sự đồng thuận', 'There is growing scientific consensus on the causes of climate change.', 7, 'agreement,accord', 'disagreement,dissent', NULL),
(67, 'fluctuation', '/ˌflʌktʃuˈeɪʃn/', 'noun', 'economics', 2, 'Sự biến động, dao động', 'Currency fluctuations can significantly affect export competitiveness.', 7, 'variation,oscillation', 'stability,constancy', NULL),
(68, 'accessibility', '/əkˌsesəˈbɪləti/', 'noun', 'society', 3, 'Khả năng tiếp cận', 'Improving accessibility to education remains a global priority.', 6, 'availability,openness', 'inaccessibility,exclusion', NULL),
(69, 'predominant', '/prɪˈdɒmɪnənt/', 'adjective', 'general', 2, 'Chiếm ưu thế, chủ yếu', 'Agriculture is the predominant industry in many developing countries.', 7, 'dominant,prevailing', 'minor,secondary', NULL),
(70, 'meticulous', '/məˈtɪkjələs/', 'adjective', 'general', 2, 'Tỉ mỉ, cẩn thận', 'Scientific research requires meticulous attention to methodology.', 8, 'thorough,precise', 'careless,sloppy', NULL),
(71, 'catastrophic', '/ˌkætəˈstrɒfɪk/', 'adjective', 'environment', 2, 'Thảm khốc, tàn khốc', 'Climate scientists warn of catastrophic consequences if temperatures rise by 3°C.', 7, 'disastrous,devastating', 'beneficial,minor', NULL),
(72, 'stigma', '/ˈstɪɡmə/', 'noun', 'society', 2, 'Sự kỳ thị, ô danh', 'Mental health stigma prevents many people from seeking treatment.', 7, 'shame,disgrace', 'honour,pride', NULL),
(73, 'trajectory', '/trəˈdʒektəri/', 'noun', 'science', 2, 'Quỹ đạo, xu hướng', 'Current emission trajectories suggest a temperature rise of 2.7°C.', 8, 'path,course', 'stagnation', NULL),
(74, 'propagate', '/ˈprɒpəɡeɪt/', 'verb', 'science', 2, 'Lan truyền, phổ biến', 'Social media can propagate misinformation rapidly.', 8, 'spread,disseminate', 'suppress,contain', NULL),
(75, 'correlation', '/ˌkɒrəˈleɪʃn/', 'noun', 'science', 3, 'Mối tương quan', 'There is a strong correlation between education level and income.', 7, 'connection,relationship', 'independence,disconnection', NULL),
(76, 'juxtapose', '/ˌdʒʌkstəˈpəʊz/', 'verb', 'culture', 2, 'Đặt cạnh nhau để so sánh', 'The essay juxtaposes traditional and modern teaching methods.', 8, 'compare,contrast', 'separate,isolate', NULL),
(77, 'exorbitant', '/ɪɡˈzɔːbɪtənt/', 'adjective', 'economics', 2, 'Quá cao, cắt cổ', 'Exorbitant housing costs force many young people to rent indefinitely.', 8, 'excessive,extortionate', 'reasonable,affordable', NULL),
(78, 'complacency', '/kəmˈpleɪsnsi/', 'noun', 'society', 2, 'Sự tự mãn, thỏa mãn', 'Complacency about climate change delays necessary action.', 8, 'self-satisfaction,smugness', 'vigilance,concern', NULL),
(79, 'tangible', '/ˈtændʒəbl/', 'adjective', 'general', 3, 'Hữu hình, cụ thể', 'The policy produced tangible improvements in air quality.', 7, 'concrete,real', 'intangible,abstract', NULL),
(80, 'substantiate', '/səbˈstænʃieɪt/', 'verb', 'science', 2, 'Chứng minh, xác thực', 'Researchers must substantiate claims with empirical evidence.', 8, 'verify,confirm', 'disprove,refute', NULL),
(81, 'deteriorating', '/dɪˈtɪəriəreɪtɪŋ/', 'adjective', 'environment', 2, 'Đang suy giảm', 'Deteriorating water quality threatens public health.', 7, 'worsening,declining', 'improving', NULL),
(82, 'simultaneously', '/ˌsɪmlˈteɪniəsli/', 'adverb', 'general', 3, 'Đồng thời', 'The experiment tested two variables simultaneously.', 6, 'concurrently,at the same time', 'separately,sequentially', NULL),
(83, 'prohibit', '/prəˈhɪbɪt/', 'verb', 'society', 3, 'Cấm, ngăn cấm', 'Several countries prohibit the use of single-use plastics.', 6, 'ban,forbid', 'allow,permit', NULL),
(84, 'hypothetical', '/ˌhaɪpəˈθetɪkl/', 'adjective', 'science', 2, 'Giả thuyết, giả định', 'The study considered hypothetical scenarios for future population growth.', 8, 'theoretical,supposed', 'actual,real', NULL),
(85, 'assimilate', '/əˈsɪmɪleɪt/', 'verb', 'society', 2, 'Hòa nhập, tiếp thu', 'Immigrants often need time to assimilate into a new culture.', 7, 'integrate,absorb', 'isolate,reject', NULL),
(86, 'proliferate', '/prəˈlɪfəreɪt/', 'verb', 'technology', 2, 'Tăng nhanh, sinh sôi', 'Online learning platforms have proliferated since the pandemic.', 8, 'multiply,spread', 'decrease,diminish', NULL),
(87, 'encapsulate', '/ɪnˈkæpsjuleɪt/', 'verb', 'general', 2, 'Tóm gọn, bao quát', 'This paragraph encapsulates the main argument of the essay.', 8, 'summarise,capture', 'expand,elaborate', NULL),
(88, 'pertinent', '/ˈpɜːtɪnənt/', 'adjective', 'general', 2, 'Thích đáng, liên quan', 'The study raises several pertinent questions about data privacy.', 7, 'relevant,applicable', 'irrelevant,unrelated', NULL),
(89, 'extrapolate', '/ɪkˈstræpəleɪt/', 'verb', 'science', 2, 'Ngoại suy, suy ra', 'We cannot simply extrapolate current trends into the distant future.', 8, 'infer,project', 'interpolate', NULL),
(90, 'ramification', '/ˌræmɪfɪˈkeɪʃn/', 'noun', 'society', 2, 'Hệ quả, nhánh ảnh hưởng', 'The policy has far-reaching ramifications for the healthcare system.', 8, 'consequence,implication', 'cause,origin', NULL),
(91, 'dichotomy', '/daɪˈkɒtəmi/', 'noun', 'society', 2, 'Sự phân đôi, đối lập', 'The dichotomy between economic growth and environmental protection is often overstated.', 8, 'divide,contrast', 'unity,agreement', NULL),
(92, 'benchmark', '/ˈbentʃmɑːk/', 'noun', 'general', 3, 'Tiêu chuẩn đánh giá', 'IELTS is widely used as a benchmark for English proficiency.', 6, 'standard,reference point', 'anomaly', NULL);


--
-- Additional Gamification Achievements
--
INSERT INTO `tb_gamification` (`achievement_id`, `achievement_code`, `achievement_name`, `description`, `icon_url`, `points`, `badge_color`, `criteria_json`, `is_active`, `created_at`) VALUES
(11, 'DAILY_STREAK_30', 'Monthly Marathoner', 'Maintain a 30-day study streak', 'https://example.com/icons/streak-30.png', 1000, 'gold', '{"type":"streak_days","count":30}', 1, '2026-05-26 12:00:00'),
(12, 'TEST_COMPLETER', 'Test Taker', 'Complete your first full practice test', 'https://example.com/icons/test-taker.png', 200, 'blue', '{"type":"test_completed","count":1}', 1, '2026-05-26 12:00:00'),
(13, 'BAND_7_OVERALL', 'Band 7 Achiever', 'Achieve an overall Band 7.0+ in any test', 'https://example.com/icons/band7.png', 1000, 'platinum', '{"type":"overall_band","min_score":7.0}', 1, '2026-05-26 12:00:00'),
(14, 'VOCAB_SCHOLAR', 'Vocabulary Scholar', 'Learn 50 vocabulary words', 'https://example.com/icons/vocab-scholar.png', 500, 'green', '{"type":"vocab_learned","count":50}', 1, '2026-05-26 12:00:00'),
(15, 'CAM20_MASTER', 'Cambridge 20 Master', 'Complete all four Cambridge IELTS 20 tests', 'https://example.com/icons/cam20-master.png', 2000, 'diamond', '{"type":"cambridge20_tests","count":4}', 1, '2026-05-26 12:00:00');

COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

SET FOREIGN_KEY_CHECKS = 1;
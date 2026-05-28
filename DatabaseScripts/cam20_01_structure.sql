-- Cambridge IELTS 20 - Structure (Course/Module/Lesson/Test)
-- Run this FIRST

-- Course
INSERT INTO `tb_courses` (`course_id`, `title`, `target_band`, `skill_type`, `difficulty_level`, `thumbnail_url`, `estimated_hours`, `is_published`, `order_index`, `slug`, `description`) VALUES
(2001, 'Cambridge IELTS 20 - Academic Test 1', 7.0, 'mixed', 3, '/images/cam20.jpg', 3, 1, 30, 'cambridge-ielts-20-test-1', 'Full academic practice test from Cambridge IELTS 20 with Listening, Reading, Writing and Speaking.');

-- Modules (4 skills)
INSERT INTO `tb_modules` (`module_id`, `course_id`, `title`, `order_index`, `is_deleted`, `deleted_at`, `updated_at`) VALUES
(2001, 2001, 'Listening - Test 1', 1, 0, NULL, NOW()),
(2002, 2001, 'Reading - Test 1', 2, 0, NULL, NOW()),
(2003, 2001, 'Writing - Test 1', 3, 0, NULL, NOW()),
(2004, 2001, 'Speaking - Test 1', 4, 0, NULL, NOW());

-- Lessons (4 listening + 3 reading + 2 writing + 3 speaking = 12)
INSERT INTO `tb_lessons` (`lesson_id`, `module_id`, `title`, `skill_type`, `difficulty_level`, `estimated_minutes`, `is_deleted`, `deleted_at`, `updated_at`) VALUES
(2001, 2001, 'Listening Section 1: Fitness Club Enquiry', 'listening', 2, 8, 0, NULL, NOW()),
(2002, 2001, 'Listening Section 2: Museum Tour Guide', 'listening', 2, 8, 0, NULL, NOW()),
(2003, 2001, 'Listening Section 3: Research Project Discussion', 'listening', 3, 10, 0, NULL, NOW()),
(2004, 2001, 'Listening Section 4: Coral Reef Conservation', 'listening', 4, 10, 0, NULL, NOW()),
(2005, 2002, 'Reading Passage 1: Urban Green Spaces', 'reading', 2, 20, 0, NULL, NOW()),
(2006, 2002, 'Reading Passage 2: Sleep and Learning', 'reading', 3, 20, 0, NULL, NOW()),
(2007, 2002, 'Reading Passage 3: AI in Healthcare', 'reading', 4, 20, 0, NULL, NOW()),
(2008, 2003, 'Writing Task 1: Internet Usage Chart', 'writing', 3, 20, 0, NULL, NOW()),
(2009, 2003, 'Writing Task 2: Foreign Language Education', 'writing', 3, 40, 0, NULL, NOW()),
(2010, 2004, 'Speaking Part 1: Food and Cooking', 'speaking', 2, 5, 0, NULL, NOW()),
(2011, 2004, 'Speaking Part 2: Helping Someone', 'speaking', 3, 4, 0, NULL, NOW()),
(2012, 2004, 'Speaking Part 3: Volunteering', 'speaking', 4, 5, 0, NULL, NOW());

-- Lesson contents
INSERT INTO `tb_lesson_contents` (`content_id`, `lesson_id`, `content_type`, `content_body`) VALUES
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

-- Tests
INSERT INTO `tb_tests` (`test_id`, `title`, `difficulty`, `duration_minutes`, `is_deleted`, `deleted_at`, `updated_at`) VALUES
(2001, 'Cambridge 20 - Listening Test 1', 3, 30, 0, NULL, NOW()),
(2002, 'Cambridge 20 - Reading Test 1', 3, 60, 0, NULL, NOW()),
(2003, 'Cambridge 20 - Writing Test 1', 3, 60, 0, NULL, NOW()),
(2004, 'Cambridge 20 - Speaking Test 1', 3, 14, 0, NULL, NOW()),
(2005, 'Cambridge 20 - Full Academic Test 1', 4, 174, 0, NULL, NOW());

-- Test sections
INSERT INTO `tb_test_sections` (`section_id`, `test_id`, `skill_type`) VALUES
(2001, 2001, 'listening'),
(2002, 2002, 'reading'),
(2003, 2003, 'writing'),
(2004, 2004, 'speaking'),
(2005, 2005, 'listening'),
(2006, 2005, 'reading'),
(2007, 2005, 'writing'),
(2008, 2005, 'speaking');

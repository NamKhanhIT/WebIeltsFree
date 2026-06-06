-- Cambridge IELTS 20 - Writing & Speaking Data
-- Run AFTER cam20_01_structure.sql

-- Writing Prompts (2 Task 1 + 2 Task 2)
INSERT INTO `tb_writing_prompts` (`prompt_id`, `task_type`, `prompt_text`, `prompt_image_url`, `chart_type`, `difficulty_level`, `category`, `band_target`, `sample_answer`, `notes_for_teacher`, `is_active`, `created_at`, `updated_at`, `status`, `created_by`, `reviewed_by`, `reviewed_at`, `reviewer_note`, `is_deleted`) VALUES
(2001, 'task1',
'The bar chart below shows the percentage of people in three age groups (18-30, 31-50, 51+) who used the internet daily in four countries in 2023. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.',
NULL, 'bar', 3, 'technology', 7,
'The bar chart compares daily internet usage across three age groups in four countries in 2023. Overall, younger people used the internet more frequently in all nations, though the gap between age groups varied significantly by country.\n\nIn Country A and Country B, the 18-30 age group had the highest usage at 95% and 91% respectively, while the over-51 group lagged behind at 62% and 58%. Country C showed the most even distribution, with all three groups falling between 70% and 82%. Country D had the widest gap: 93% of young adults used the internet daily compared to just 41% of those aged 51 and over.\n\nIn summary, while internet adoption was high among young adults everywhere, digital engagement among older populations varied considerably between nations.',
'Check overview quality, data selection, and comparative language.', 1, NOW(), NOW(), 'published', NULL, NULL, NULL, NULL, 0),

(2002, 'task1',
'The two maps below show a small coastal town in 1990 and the same town in 2025. Summarise the information by selecting and reporting the main features, and make comparisons where relevant.',
NULL, 'map', 3, 'urban development', 7,
'The maps illustrate changes to a coastal town between 1990 and 2025. Overall, the town underwent substantial modernisation, with industrial areas replaced by leisure facilities and improved transport links.\n\nIn 1990 the northern area contained a fish processing factory and warehouse, while the southern coastline had a small beach and a row of fishermen''s cottages. By 2025, the factory site had been redeveloped into a hotel complex with a swimming pool, and the warehouse was converted into a shopping centre. The cottages were demolished and replaced by a marina with boat moorings.\n\nA new coastal road was built connecting the eastern residential area to the marina, and a car park was added near the hotel. The only feature that remained unchanged was the church in the town centre.',
'Evaluate spatial grouping and use of change vocabulary.', 1, NOW(), NOW(), 'published', NULL, NULL, NULL, NULL, 0),

(2003, 'task2',
'Some people believe that children should begin learning a foreign language in primary school rather than in secondary school. To what extent do you agree or disagree?',
NULL, NULL, 3, 'education', 7,
'Starting foreign language instruction in primary school offers clear cognitive and practical advantages, and I strongly agree that early introduction is preferable.\n\nYoung children possess a natural capacity for language acquisition. Research shows that the brain''s neural plasticity peaks before age ten, making pronunciation and grammar patterns easier to absorb. Children who begin bilingual education at age six typically achieve near-native fluency by their mid-teens, whereas those who start at twelve rarely reach equivalent pronunciation accuracy.\n\nFurthermore, early language learning develops broader cognitive skills. Studies have linked childhood bilingualism to improved executive function, enhanced problem-solving ability, and greater cultural empathy. These benefits compound over time and are difficult to replicate through later instruction alone.\n\nCritics argue that primary curricula are already crowded and that children should first master their mother tongue. However, evidence from countries such as the Netherlands and Singapore, where multilingual education begins at age five, shows no negative effect on first-language proficiency.\n\nIn conclusion, the cognitive window for language learning is limited, and delaying instruction to secondary school wastes a valuable developmental opportunity.',
'Check thesis clarity, argument depth, and conclusion consistency.', 1, NOW(), NOW(), 'published', NULL, NULL, NULL, NULL, 0),

(2004, 'task2',
'In many countries, the gap between the cost of living and average wages is growing. What problems does this cause, and what measures could governments take to address the issue?',
NULL, NULL, 3, 'economics', 7,
'The widening gap between living costs and wages creates serious social problems that require coordinated government intervention.\n\nThe most immediate consequence is housing insecurity. When rent or mortgage payments consume more than forty percent of household income, families are forced to sacrifice spending on nutrition, healthcare and education. This in turn creates a cycle of disadvantage that is difficult to escape.\n\nA second problem is reduced consumer spending. When disposable income shrinks, retail and service sectors contract, leading to job losses and slower economic growth. The resulting anxiety can also affect mental health, with studies linking financial stress to higher rates of depression.\n\nGovernments can respond in several ways. First, raising the minimum wage to a genuine living wage ensures that full-time workers can meet basic needs. Second, investing in affordable public housing reduces the largest single expense for low-income households. Third, targeted subsidies for childcare and transport can free up income for other essentials.\n\nIn conclusion, the cost-of-living crisis demands practical, multi-level policy responses rather than reliance on market self-correction alone.',
'Check cause-solution structure and feasibility of proposals.', 1, NOW(), NOW(), 'published', NULL, NULL, NULL, NULL, 0);

-- Speaking Topics (3 topics for Parts 1, 2, 3)
INSERT INTO `tb_speaking_topics` (`topic_id`, `part`, `topic_title`, `description`, `difficulty_level`, `band_target`, `is_active`, `created_at`, `updated_at`, `status`, `created_by`, `reviewed_by`, `reviewed_at`, `reviewer_note`, `is_deleted`) VALUES
(2001, '1', 'Food and cooking habits',
'Questions about what you eat, whether you cook, and food preferences in your country.', 2, 6, 1, NOW(), NOW(), 'published', NULL, NULL, NULL, NULL, 0),
(2002, '2', 'A time you helped someone',
'Describe a time when you helped someone who really needed it. You should say who the person was, what the situation was, how you helped, and explain how you felt afterwards.', 3, 7, 1, NOW(), NOW(), 'published', NULL, NULL, NULL, NULL, 0),
(2003, '3', 'Volunteering and community service',
'Discussion questions about the value of volunteering, whether it should be compulsory, and how communities benefit from unpaid work.', 4, 7, 1, NOW(), NOW(), 'published', NULL, NULL, NULL, NULL, 0);

-- Speaking Topic Parts (questions within each topic)
INSERT INTO `tb_speaking_topic_parts` (`part_id`, `topic_id`, `part_number`, `content_text`, `time_limit_seconds`, `sequence_order`, `is_follow_up`) VALUES
(2001, 2001, 1, 'Do you enjoy cooking? Why or why not?', 35, 1, 0),
(2002, 2001, 1, 'What is a typical meal in your country?', 35, 2, 1),
(2003, 2001, 1, 'Do you think people eat more healthily now than in the past?', 35, 3, 1),
(2004, 2002, 2, 'You should say: who you helped, what the problem was, what you did to help, and explain how you felt about it afterwards.', 120, 1, 0),
(2005, 2002, 2, 'Would you help that person again in the same way?', 35, 2, 1),
(2006, 2003, 3, 'Why do some people volunteer while others do not?', 70, 1, 0),
(2007, 2003, 3, 'Should schools require students to do community service?', 70, 2, 1),
(2008, 2003, 3, 'How can governments encourage more people to volunteer?', 70, 3, 1);

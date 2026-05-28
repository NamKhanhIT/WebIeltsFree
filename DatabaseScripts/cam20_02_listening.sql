-- Cambridge IELTS 20 - Listening Data
-- Run AFTER cam20_01_structure.sql

-- Listening Materials (4 sections)
INSERT INTO `tb_listening_materials` (`material_id`, `lesson_id`, `material_type`, `title`, `transcript`, `audio_url`, `duration_seconds`, `speaker_count`, `topics`, `notes_for_learner`, `difficulty_level`, `source`, `created_at`) VALUES
(2001, 2001, 'conversation', 'Fitness Club Membership Enquiry',
'Receptionist: Good morning, Riverside Fitness Club. How can I help?\nCaller: Hi, I''d like to ask about membership options.\nReceptionist: Of course. We have three tiers: Bronze at forty-five pounds per month, Silver at sixty-five, and Gold at eighty-five.\nCaller: What does Gold include?\nReceptionist: Unlimited gym access, pool, all group classes, and a personal training session every month. Plus free parking.\nCaller: That sounds good. My name is Katherine Bromley. That''s B-R-O-M-L-E-Y.\nReceptionist: And your date of birth?\nCaller: The fourteenth of March, nineteen ninety-two.\nReceptionist: Great. We''re open weekdays six AM to ten PM, and weekends eight to six. Your induction is booked for next Tuesday at half past nine.',
'/audio/cam20/T1S1.m4a', 300, 2, 'fitness,membership,booking', 'Listen for corrected numbers, spelling, and times.', 'band_5_6', 'Cambridge IELTS 20 Academic', NOW()),

(2002, 2002, 'monologue', 'City Museum Visitor Information',
'Welcome to the City Heritage Museum. Before we start the tour, let me explain the layout. You''re currently in the Main Hall. Directly ahead is the Ancient History gallery. If you turn left past the gift shop, you''ll find the Science Wing, which opened last September. The café is on the ground floor, next to the east entrance. Toilets are located behind the information desk. The special exhibition on marine archaeology is on the second floor, running until the end of November. Photography is permitted in all areas except the Manuscript Room. We ask that you keep your voices low in the Reading Room on the third floor.',
'/audio/cam20/T1S2.m4a', 330, 1, 'museum,directions,facilities', 'Track spatial language and location references.', 'band_5_6', 'Cambridge IELTS 20 Academic', NOW()),

(2003, 2003, 'discussion', 'Research Assignment: Water Conservation Study',
'Tutor: So, how is your water conservation project coming along?\nSara: We''ve finished the literature review. The main challenge is the survey design.\nMark: I think we should use a stratified sample rather than random, because the campus population is quite diverse.\nTutor: That''s sensible. What about your sample size?\nSara: We''re aiming for a hundred and fifty responses. Mark wanted two hundred but the deadline is in three weeks.\nMark: True. And we need to decide on the analysis software. I''ve used SPSS before but Sara prefers Python.\nTutor: Either works. The key thing is consistency. Also, have you considered the ethical approval? You''ll need to submit the form by Friday.\nSara: Yes, we''ve drafted the consent form already.',
'/audio/cam20/T1S3.m4a', 380, 3, 'research,education,methodology', 'Match opinions to speakers and follow the discussion flow.', 'band_6_7', 'Cambridge IELTS 20 Academic', NOW()),

(2004, 2004, 'lecture', 'Threats to Coral Reef Ecosystems',
'Today I want to discuss the decline of coral reef ecosystems worldwide. Coral reefs support approximately twenty-five percent of all marine species despite covering less than one percent of the ocean floor. The primary threat is ocean warming, which causes coral bleaching. When water temperatures rise by just one to two degrees Celsius above the summer maximum, corals expel their symbiotic algae and turn white. If conditions persist beyond six weeks, mortality rates can exceed seventy percent. A second major factor is ocean acidification, caused by increased CO2 absorption. The pH of surface oceans has dropped by 0.1 units since pre-industrial times. Additionally, agricultural runoff introduces excess nutrients that promote algal blooms, blocking sunlight. Conservation strategies include establishing marine protected areas, reducing land-based pollution, and coral nursery programs where fragments are grown on underwater frames before transplanting to damaged reefs.',
'/audio/cam20/T1S4.m4a', 420, 1, 'environment,marine-biology,conservation', 'Note signpost words and numerical data carefully.', 'band_6_7', 'Cambridge IELTS 20 Academic', NOW());

-- Listening Questions (5 per section = 20 total)
INSERT INTO `tb_listening_questions` (`question_id`, `material_id`, `question_type`, `question_text`, `time_code_start`, `time_code_end`, `correct_answer`, `options`, `explanation`, `band_target`, `question_order`, `created_at`) VALUES
-- Section 1 (form completion / MCQ)
(2001, 2001, 'form_completion', 'Monthly cost of Gold membership: £____', 10, 25, '85', NULL, 'Receptionist says eighty-five pounds.', 5.5, 1, NOW()),
(2002, 2001, 'form_completion', 'Caller surname spelling: ____', 30, 45, 'Bromley', NULL, 'Spelled out B-R-O-M-L-E-Y.', 5.5, 2, NOW()),
(2003, 2001, 'form_completion', 'Date of birth: 14th ____ 1992', 48, 60, 'March', NULL, 'Stated as fourteenth of March.', 5.5, 3, NOW()),
(2004, 2001, 'form_completion', 'Weekend closing time: ____', 65, 78, '6 PM', NULL, 'Weekends eight to six.', 5.5, 4, NOW()),
(2005, 2001, 'form_completion', 'Induction time: ____', 80, 95, '9:30', NULL, 'Half past nine on Tuesday.', 6.0, 5, NOW()),
-- Section 2 (map labelling / note completion)
(2006, 2002, 'note_completion', 'The Science Wing opened in ____.', 15, 30, 'September', NULL, 'Opened last September.', 6.0, 6, NOW()),
(2007, 2002, 'map_labelling', 'The café is next to the ____ entrance.', 32, 48, 'east', NULL, 'Next to the east entrance.', 6.0, 7, NOW()),
(2008, 2002, 'note_completion', 'The marine archaeology exhibition ends in ____.', 50, 65, 'November', NULL, 'Running until end of November.', 6.0, 8, NOW()),
(2009, 2002, 'multiple_choice', 'Photography is NOT allowed in:', 68, 80, 'C', '["A. Ancient History gallery","B. Science Wing","C. Manuscript Room","D. Main Hall"]', 'All areas except the Manuscript Room.', 6.0, 9, NOW()),
(2010, 2002, 'note_completion', 'Visitors should keep quiet in the ____ Room.', 82, 95, 'Reading', NULL, 'Keep voices low in the Reading Room.', 5.5, 10, NOW()),
-- Section 3 (matching / MCQ)
(2011, 2003, 'multiple_choice', 'What sampling method does Mark prefer?', 18, 35, 'B', '["A. Random","B. Stratified","C. Convenience","D. Cluster"]', 'Mark suggests stratified sample.', 6.5, 11, NOW()),
(2012, 2003, 'form_completion', 'Target number of survey responses: ____', 38, 50, '150', NULL, 'Aiming for a hundred and fifty.', 6.5, 12, NOW()),
(2013, 2003, 'form_completion', 'Project deadline is in ____ weeks.', 52, 64, '3', NULL, 'Deadline is in three weeks.', 6.5, 13, NOW()),
(2014, 2003, 'matching', 'Sara prefers using ____ for analysis.', 66, 78, 'Python', NULL, 'Sara prefers Python.', 6.5, 14, NOW()),
(2015, 2003, 'short_answer', 'Ethical approval form due by ____.', 80, 95, 'Friday', NULL, 'Submit the form by Friday.', 6.5, 15, NOW()),
-- Section 4 (note completion / summary)
(2016, 2004, 'note_completion', 'Coral reefs support ____% of marine species.', 14, 28, '25', NULL, 'Twenty-five percent stated.', 7.0, 16, NOW()),
(2017, 2004, 'summary_completion', 'Bleaching occurs when temperature rises ____ degrees above maximum.', 30, 48, '1 to 2', NULL, 'One to two degrees Celsius.', 7.0, 17, NOW()),
(2018, 2004, 'note_completion', 'Mortality can exceed ____% if bleaching persists beyond 6 weeks.', 50, 65, '70', NULL, 'Seventy percent mortality rate.', 7.0, 18, NOW()),
(2019, 2004, 'summary_completion', 'Ocean pH has dropped by ____ units since pre-industrial times.', 68, 82, '0.1', NULL, 'pH drop of 0.1 units.', 7.0, 19, NOW()),
(2020, 2004, 'note_completion', 'In coral nurseries, fragments are grown on underwater ____.', 85, 100, 'frames', NULL, 'Grown on underwater frames.', 7.0, 20, NOW());

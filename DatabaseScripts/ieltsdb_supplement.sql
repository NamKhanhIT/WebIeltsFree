-- WebIeltsFree Supplementary SQL Data
-- Optimized for phpMyAdmin / MariaDB / MySQL
--
USE `ieltsdb`;

SET FOREIGN_KEY_CHECKS = 0;
SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

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
-- Pawsitive Haven Database Schema
-- PostgreSQL 17

-- Drop tables if they exist (for clean rebuilds)
DROP TABLE IF EXISTS notification_preferences CASCADE;
DROP TABLE IF EXISTS medical_records CASCADE;
DROP TABLE IF EXISTS pet_photos CASCADE;
DROP TABLE IF EXISTS conversation_messages CASCADE;
DROP TABLE IF EXISTS conversations CASCADE;
DROP TABLE IF EXISTS appointments CASCADE;
DROP TABLE IF EXISTS pets CASCADE;
DROP TABLE IF EXISTS faqs CASCADE;
DROP TABLE IF EXISTS users CASCADE;

-- Users table
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    user_level VARCHAR(20) NOT NULL DEFAULT 'User',
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create index for login queries
CREATE INDEX idx_users_username ON users(username);
CREATE INDEX idx_users_email ON users(email);

-- Pets table
CREATE TABLE pets (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL,
    species VARCHAR(50) NOT NULL,
    breed VARCHAR(100),
    age INTEGER,
    sex VARCHAR(20),
    bio TEXT,
    image_url VARCHAR(500),
    foster_id INTEGER REFERENCES users(id) ON DELETE SET NULL,
    assigned_at TIMESTAMP WITH TIME ZONE,
    assignment_notes TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create index for user pet queries
CREATE INDEX idx_pets_user_id ON pets(user_id);
CREATE INDEX idx_pets_foster_id ON pets(foster_id);

-- Pet photos table
CREATE TABLE pet_photos (
    id SERIAL PRIMARY KEY,
    pet_id INTEGER NOT NULL REFERENCES pets(id) ON DELETE CASCADE,
    file_name VARCHAR(255) NOT NULL,
    file_path VARCHAR(500) NOT NULL,
    is_primary BOOLEAN NOT NULL DEFAULT FALSE,
    uploaded_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    uploaded_by INTEGER REFERENCES users(id) ON DELETE SET NULL
);

-- Create indexes for pet photos
CREATE INDEX idx_pet_photos_pet_id ON pet_photos(pet_id);
CREATE INDEX idx_pet_photos_is_primary ON pet_photos(pet_id, is_primary);

-- Medical records table (for tracking vaccinations, vet visits, medications, surgeries)
CREATE TABLE medical_records (
    id SERIAL PRIMARY KEY,
    pet_id INTEGER NOT NULL REFERENCES pets(id) ON DELETE CASCADE,
    record_type VARCHAR(50) NOT NULL, -- 'Vaccination', 'VetVisit', 'Medication', 'Surgery', 'Other'
    title VARCHAR(200) NOT NULL,
    description TEXT,
    record_date DATE NOT NULL,
    next_due_date DATE, -- for vaccinations/medications that need to be repeated
    veterinarian VARCHAR(100),
    clinic_name VARCHAR(100),
    cost DECIMAL(10,2),
    notes TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by INTEGER REFERENCES users(id) ON DELETE SET NULL
);

-- Create indexes for medical records queries
CREATE INDEX idx_medical_records_pet_id ON medical_records(pet_id);
CREATE INDEX idx_medical_records_record_type ON medical_records(record_type);
CREATE INDEX idx_medical_records_next_due_date ON medical_records(next_due_date);
CREATE INDEX idx_medical_records_record_date ON medical_records(record_date);

-- Appointments table
CREATE TABLE appointments (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    pet_id INTEGER REFERENCES pets(id) ON DELETE SET NULL,
    title VARCHAR(200) NOT NULL,
    description TEXT,
    appointment_date DATE NOT NULL,
    appointment_time TIME,
    is_completed BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create indexes for appointment queries
CREATE INDEX idx_appointments_user_id ON appointments(user_id);
CREATE INDEX idx_appointments_date ON appointments(appointment_date);

-- FAQs table
CREATE TABLE faqs (
    id SERIAL PRIMARY KEY,
    question TEXT NOT NULL,
    answer TEXT NOT NULL,
    display_order INTEGER NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create index for active FAQs
CREATE INDEX idx_faqs_active ON faqs(is_active, display_order);

-- Conversations table (for AI chat history)
CREATE TABLE conversations (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    title VARCHAR(200),
    openai_thread_id VARCHAR(100),
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create index for user conversations
CREATE INDEX idx_conversations_user_id ON conversations(user_id);

-- Conversation messages table
CREATE TABLE conversation_messages (
    id SERIAL PRIMARY KEY,
    conversation_id INTEGER NOT NULL REFERENCES conversations(id) ON DELETE CASCADE,
    role VARCHAR(20) NOT NULL, -- 'user', 'assistant', 'system'
    content TEXT NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create index for conversation messages
CREATE INDEX idx_conversation_messages_conversation_id ON conversation_messages(conversation_id);

-- Notification preferences table
CREATE TABLE notification_preferences (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE UNIQUE,
    email_appointments BOOLEAN NOT NULL DEFAULT TRUE,
    email_reminders BOOLEAN NOT NULL DEFAULT TRUE,
    reminder_days_before INTEGER NOT NULL DEFAULT 1,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create index for notification preferences
CREATE INDEX idx_notification_preferences_user_id ON notification_preferences(user_id);

-- Escalations table (for human support requests)
CREATE TABLE escalations (
    id SERIAL PRIMARY KEY,
    conversation_id INTEGER NOT NULL REFERENCES conversations(id) ON DELETE CASCADE,
    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    message_id INTEGER REFERENCES conversation_messages(id) ON DELETE SET NULL,
    user_email VARCHAR(255) NOT NULL,
    user_name VARCHAR(100) NOT NULL,
    user_question TEXT NOT NULL,
    additional_context TEXT,
    status VARCHAR(20) NOT NULL DEFAULT 'Pending',
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    email_sent_at TIMESTAMP WITH TIME ZONE,
    resolved_at TIMESTAMP WITH TIME ZONE,
    staff_notes TEXT
);

-- Create indexes for escalations
CREATE INDEX idx_escalations_status ON escalations(status);
CREATE INDEX idx_escalations_user_id ON escalations(user_id);
CREATE INDEX idx_escalations_conversation_id ON escalations(conversation_id);

-- Insert default admin user (password: Test12345)
-- BCrypt hash for 'Test12345' with work factor 12
INSERT INTO users (username, email, password_hash, user_level) VALUES
('admin', 'admin@pawsitivehaven.com', '$2a$12$wezPYwqGlaMUUdaRIOSuw.68/M/3oclTprLG10gEl7JGzazsc2yRq', 'Admin');

-- Insert demo user (password: Test12345)
INSERT INTO users (username, email, password_hash, user_level) VALUES
('demo', 'demo@pawsitivehaven.com', '$2a$12$wezPYwqGlaMUUdaRIOSuw.68/M/3oclTprLG10gEl7JGzazsc2yRq', 'User');

-- Insert sample pets for demo user
INSERT INTO pets (user_id, name, species, breed, age, sex, bio) VALUES
(2, 'Buddy', 'Dog', 'Golden Retriever', 3, 'Male', 'A friendly and energetic golden retriever who loves to play fetch and swim.'),
(2, 'Whiskers', 'Cat', 'Tabby', 5, 'Female', 'A calm and affectionate tabby cat who enjoys sunny spots and gentle pets.');

-- Insert sample FAQs - Pawsitive Haven Pet Rescue
INSERT INTO faqs (question, answer, display_order) VALUES
-- Adoption Process
('What are your adoption hours?', 'Our Adoption Center is open Thursday through Sunday, 11 AM to 5 PM. We recommend scheduling an adoption appointment for the best experience. Our main office is open Tuesday through Saturday, 10 AM to 6 PM. We are closed Sundays and Mondays for animal care and staff rest.', 1),
('What is the adoption process?', 'Our adoption process includes: 1) Browse available dogs on our website, 2) Submit an adoption application online, 3) Application review (24-72 hours), 4) Phone or video interview, 5) Meet and greet with the dog, 6) Home check if required, 7) Adoption approval, 8) Complete paperwork and pay fees at adoption appointment. The process typically takes 5-7 days.', 2),
('What are the adoption fees?', 'Adoption fees are: Puppies (under 1 year) $350, Adult dogs (1-7 years) $275, Senior dogs (8+ years) $150, Special needs dogs $150, Bonded pairs $400 for both. All dogs are spayed/neutered, vaccinated (DHPP, Bordetella, Rabies), heartworm tested, microchipped, and receive flea/tick treatment. We offer discounts for veterans, seniors adopting senior dogs, and repeat adopters.', 3),
('Can I adopt if I rent an apartment?', 'Yes! Renters are welcome to adopt. We require written landlord approval confirming pets are allowed and any breed or size restrictions. Many of our dogs thrive in apartments with proper exercise. Email your landlord approval to applications@pawsitivehaven.org along with your application.', 4),
('How long does the adoption process take?', 'From application to bringing your dog home typically takes 5-7 days. This includes application review (24-72 hours), interview scheduling, meet and greet, and adoption appointment. The timeline may vary based on your schedule, home check requirements, and dog-to-dog introductions if you have resident pets.', 5),

-- Fostering
('How do I become a foster?', 'To become a foster: 1) Fill out our foster application at pawsitivehaven.org/foster, 2) Attend a foster orientation (virtual or in-person), 3) Complete your foster profile with preferences, 4) Get matched with a foster dog. Pawsitive Haven provides supplies including crate, food, leash, collar, and covers all medical expenses. Email newfoster@pawsitivehaven.org with questions.', 6),
('What does Pawsitive Haven provide for fosters?', 'We provide: crate (appropriate size), leash and collar with ID tag, initial food supply (2-week starter bag), food and water bowls. We also cover ALL veterinary care including vaccines, spay/neuter, heartworm testing and treatment, flea/tick prevention, and any medical needs. Fosters provide love, time, and a safe space.', 7),
('How long do dogs typically stay in foster?', 'Foster length varies widely - some dogs are adopted within days, others may take weeks or months. Average stay is 2-4 weeks. Senior dogs, special needs dogs, or those requiring more training may stay longer. We work to match you with dogs that fit your timeline and communicate openly about expected duration.', 8),
('What is the 3-3-3 rule for foster dogs?', 'The 3-3-3 rule describes a rescue dog''s adjustment period: First 3 DAYS - Dog may be overwhelmed, shut down, not eat, or not show true personality. First 3 WEEKS - Dog starts settling, learning routine, personality begins emerging. First 3 MONTHS - Dog is fully comfortable, true personality shows, bonded with family. Be patient during this adjustment!', 9),
('Can I foster if I have other pets?', 'Yes! Many of our fosters have resident pets. We''ll match you with dogs that are appropriate for your household. Dog introductions should be slow and supervised - we recommend waiting 3-5 days before any introductions. Cat households need dogs listed as cat-safe, and introductions must be very gradual. See our First Time Foster Guide for details.', 10),

-- Medical & Care
('What vaccinations do adopted dogs receive?', 'All Pawsitive Haven dogs receive: DHPP (Distemper, Hepatitis, Parainfluenza, Parvovirus), Bordetella (kennel cough), and Rabies vaccines. They are also heartworm tested (and treated if positive), spayed or neutered, microchipped, and receive flea/tick treatment. You''ll receive complete medical records at adoption.', 11),
('What should I do if my foster dog gets sick?', 'For non-emergencies: Email vetappointments@pawsitivehaven.org to schedule an appointment. For urgent issues (persistent vomiting, not eating 24+ hours, lethargy): Email marked URGENT or call the vet line. For EMERGENCIES (difficulty breathing, severe bleeding, collapse, bloat): Call (555) PAW-VET1 immediately or go directly to the emergency vet.', 12),
('Does Pawsitive Haven cover medical expenses for fosters?', 'Yes! Pawsitive Haven covers 100% of medical expenses for foster dogs including: routine vet visits, vaccinations, spay/neuter surgery, heartworm testing and treatment, flea/tick prevention, medications, and emergency care. Simply schedule through vetappointments@pawsitivehaven.org. Do not take your foster to your personal vet without prior approval.', 13),
('How often should I give heartworm and flea prevention?', 'Heartworm and flea/tick prevention should be given monthly, year-round. Check your foster dog''s medical records for due dates. Request refills from meds@pawsitivehaven.org at least one week before you run out. Prevention is critical - heartworm treatment is expensive and hard on dogs, and flea infestations are difficult to eliminate.', 14),
('What is the decompression period for new foster dogs?', 'The first 72 hours are the decompression period - your foster needs rest, not socialization. Keep things calm and quiet, limit interactions to potty breaks and meals, don''t introduce to other pets yet, keep on leash even in fenced yards, and let them approach you. They may not eat much or show their true personality yet. This is normal!', 15),

-- Behavior & Training
('My foster dog is having accidents inside. What should I do?', 'House training takes time, especially for dogs adjusting to new environments. Take outside frequently (every 2-3 hours, after meals, after naps). Always go to the same potty spot and praise when they go. Clean accidents with enzymatic cleaner. Don''t punish accidents. Crate training helps. If accidents persist after 2 weeks, email behavior@pawsitivehaven.org for support.', 16),
('How do I introduce my foster dog to my resident dog?', 'Wait minimum 3-5 days before introductions. Start with scent swapping (exchange blankets). Then parallel walks outside (separate handlers, 20+ feet apart). Gradually decrease distance. First meeting should be in neutral territory, both on leash. Keep initial meetings short and positive. Always supervise until fully trusted. See our Foster Guide for detailed steps.', 17),
('My foster dog seems scared or shut down. Is this normal?', 'Yes, this is common in the first few days to weeks. Dogs in rescue have often experienced trauma, shelter stress, or multiple transitions. Give them space, move slowly, speak softly, and let them approach you. Maintain consistent routines. Most dogs open up significantly within 1-2 weeks. Contact behavior@pawsitivehaven.org if concerns persist beyond 2 weeks.', 18),
('What training should I work on with my foster dog?', 'Focus on basics that help them get adopted: name recognition, sit, walking on leash, crate training. Keep sessions short (5 minutes), use positive reinforcement with treats, and end on success. Don''t worry about perfecting behaviors - showing potential adopters that the dog is willing to learn is most valuable.', 19),
('Who do I contact for behavior problems with my foster?', 'Email behavior@pawsitivehaven.org with specific details about the behavior (what happened, when, triggers). Our behavior team can provide guidance, resources, and training tips. For reactive dog support, email reactive@pawsitivehaven.org. For serious concerns (aggression, bite incidents), contact (555) PAW-SAFE immediately.', 20),

-- Marketing & Adoption
('How do I help my foster get adopted?', 'Great photos and an accurate bio are key! Take photos in natural light, get on their level, capture personality. Submit to photos@pawsitivehaven.org. Write a bio describing their personality, energy level, what they''re good with, and what they need. Submit to bios@pawsitivehaven.org. Share on your social media (tag @PawsitiveHavenRescue). The more visibility, the faster they find their family!', 21),
('Someone wants to adopt my foster. What do I do?', 'Great news! Direct interested adopters to apply at pawsitivehaven.org/apply. Once they''re approved, the adoption team will contact you to schedule a meet and greet. Your honest input about the dog helps ensure a good match. You may be asked to participate in the meet and greet to share insights about the dog''s personality and routine.', 22),
('What happens at a meet and greet?', 'Meet and greets allow potential adopters to interact with the dog in person. As the foster, you''ll share information about the dog''s personality, routine, quirks, and needs. Let the dog and adopter interact naturally. Answer questions honestly - finding the RIGHT match is more important than a fast adoption. Contact meetandgreet@pawsitivehaven.org to schedule.', 23),

-- Support & Resources
('Who do I contact if my foster dog escapes?', 'Call (555) PAW-LOST IMMEDIATELY. Do not wait. Provide: your location, dog''s description, direction they went, how long ago. Our lost dog team will mobilize immediately. Post on local lost pet Facebook groups and Nextdoor but coordinate with our team. Time is critical - most dogs are found within the first few hours.', 24),
('How do I get supplies for my foster dog?', 'Email supplies@pawsitivehaven.org with what you need (food, crate, leash, etc.). Include your foster dog''s name and your location. Supplies are typically available for pickup at our office or can be coordinated with volunteers for delivery. Requests are usually fulfilled within 48 hours.', 25),
('Can I take a break from fostering?', 'Absolutely! Fostering is rewarding but can be demanding. Email fostersupport@pawsitivehaven.org to update your availability. You can request a break between fosters, specify preferences (easier dogs only, short-term only, etc.), or pause fostering entirely. We appreciate every foster and want you to have a positive experience. There''s no judgment for needing time.', 26);

-- Insert sample appointments for demo user
INSERT INTO appointments (user_id, pet_id, title, description, appointment_date, appointment_time) VALUES
(2, 1, 'Annual Checkup', 'Buddy''s yearly wellness exam and vaccinations', CURRENT_DATE + INTERVAL '7 days', '10:00:00'),
(2, 2, 'Grooming', 'Whiskers'' nail trim and brushing', CURRENT_DATE + INTERVAL '3 days', '14:00:00');

-- Insert sample medical records for demo pets
INSERT INTO medical_records (pet_id, record_type, title, description, record_date, next_due_date, veterinarian, clinic_name, cost, notes, created_by) VALUES
-- Buddy's medical records
(1, 'Vaccination', 'DHPP Vaccine', 'Distemper, Hepatitis, Parainfluenza, Parvovirus vaccine', CURRENT_DATE - INTERVAL '6 months', CURRENT_DATE + INTERVAL '6 months', 'Dr. Sarah Johnson', 'Pawsitive Vet Clinic', 45.00, 'No adverse reactions observed', 2),
(1, 'Vaccination', 'Rabies Vaccine', 'Annual rabies vaccination', CURRENT_DATE - INTERVAL '3 months', CURRENT_DATE + INTERVAL '9 months', 'Dr. Sarah Johnson', 'Pawsitive Vet Clinic', 25.00, 'Required by law', 2),
(1, 'Medication', 'Heartworm Prevention', 'Monthly heartworm preventative - Heartgard Plus', CURRENT_DATE - INTERVAL '1 month', CURRENT_DATE + INTERVAL '1 month', 'Dr. Sarah Johnson', 'Pawsitive Vet Clinic', 15.00, 'Give with food on the 1st of each month', 2),
(1, 'VetVisit', 'Annual Wellness Exam', 'Complete physical examination', CURRENT_DATE - INTERVAL '6 months', CURRENT_DATE + INTERVAL '6 months', 'Dr. Sarah Johnson', 'Pawsitive Vet Clinic', 75.00, 'Healthy weight, good dental health, all vitals normal', 2),
-- Whiskers' medical records
(2, 'Vaccination', 'FVRCP Vaccine', 'Feline viral rhinotracheitis, calicivirus, panleukopenia', CURRENT_DATE - INTERVAL '4 months', CURRENT_DATE + INTERVAL '8 months', 'Dr. Mike Chen', 'City Cat Clinic', 40.00, 'Booster due in 1 year', 2),
(2, 'Vaccination', 'Rabies Vaccine', 'Annual rabies vaccination', CURRENT_DATE - INTERVAL '2 months', CURRENT_DATE + INTERVAL '10 months', 'Dr. Mike Chen', 'City Cat Clinic', 25.00, 'No adverse reactions', 2),
(2, 'VetVisit', 'Dental Cleaning', 'Professional dental cleaning under anesthesia', CURRENT_DATE - INTERVAL '2 months', NULL, 'Dr. Mike Chen', 'City Cat Clinic', 250.00, 'Minor tartar buildup removed, teeth in good condition', 2),
(2, 'Medication', 'Flea Prevention', 'Monthly flea and tick prevention - Revolution Plus', CURRENT_DATE - INTERVAL '15 days', CURRENT_DATE + INTERVAL '15 days', 'Dr. Mike Chen', 'City Cat Clinic', 20.00, 'Apply topically between shoulder blades', 2);

-- Function to update updated_at timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Create triggers for updated_at
CREATE TRIGGER update_users_updated_at BEFORE UPDATE ON users FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_pets_updated_at BEFORE UPDATE ON pets FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_appointments_updated_at BEFORE UPDATE ON appointments FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_faqs_updated_at BEFORE UPDATE ON faqs FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_conversations_updated_at BEFORE UPDATE ON conversations FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_notification_preferences_updated_at BEFORE UPDATE ON notification_preferences FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

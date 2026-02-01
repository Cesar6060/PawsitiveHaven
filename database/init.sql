-- Pawsitive Haven Database Schema
-- PostgreSQL 17

-- Drop tables if they exist (for clean rebuilds)
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
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create index for user pet queries
CREATE INDEX idx_pets_user_id ON pets(user_id);

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

-- Insert default admin user (password: Admin123!)
-- BCrypt hash for 'Admin123!' with work factor 12
INSERT INTO users (username, email, password_hash, user_level) VALUES
('admin', 'admin@pawsitivehaven.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/X4.S8F6f.5B9.xE2S', 'Admin');

-- Insert demo user (password: Demo123!)
INSERT INTO users (username, email, password_hash, user_level) VALUES
('demo', 'demo@pawsitivehaven.com', '$2a$12$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'User');

-- Insert sample pets for demo user
INSERT INTO pets (user_id, name, species, breed, age, sex, bio) VALUES
(2, 'Buddy', 'Dog', 'Golden Retriever', 3, 'Male', 'A friendly and energetic golden retriever who loves to play fetch and swim.'),
(2, 'Whiskers', 'Cat', 'Tabby', 5, 'Female', 'A calm and affectionate tabby cat who enjoys sunny spots and gentle pets.');

-- Insert sample FAQs
INSERT INTO faqs (question, answer, display_order) VALUES
('What are your adoption hours?', 'We are open for adoptions Tuesday through Sunday, 10 AM to 6 PM. We are closed on Mondays for animal care and facility maintenance.', 1),
('What is the adoption process?', 'Our adoption process includes: 1) Browse available pets online or visit us, 2) Fill out an adoption application, 3) Meet with an adoption counselor, 4) Meet your potential pet, 5) Complete adoption paperwork and fees.', 2),
('What are the adoption fees?', 'Adoption fees vary by animal: Dogs $150-300, Cats $75-150, Small animals $25-50. All animals are spayed/neutered, vaccinated, and microchipped.', 3),
('Do you offer pet surrenders?', 'Yes, we accept pet surrenders by appointment. Please call us at (555) 123-4567 to schedule a surrender appointment. We ask for a surrender fee to help cover the cost of care.', 4),
('How can I volunteer?', 'We welcome volunteers! Visit our website to fill out a volunteer application. Opportunities include dog walking, cat socialization, event support, and administrative help.', 5),
('Do you have veterinary services?', 'We provide basic veterinary care for shelter animals. For adopted pets, we recommend establishing care with a local veterinarian. We can provide referrals upon request.', 6);

-- Insert sample appointments for demo user
INSERT INTO appointments (user_id, pet_id, title, description, appointment_date, appointment_time) VALUES
(2, 1, 'Annual Checkup', 'Buddy''s yearly wellness exam and vaccinations', CURRENT_DATE + INTERVAL '7 days', '10:00:00'),
(2, 2, 'Grooming', 'Whiskers'' nail trim and brushing', CURRENT_DATE + INTERVAL '3 days', '14:00:00');

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

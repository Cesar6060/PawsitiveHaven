# Pawsitive Haven Pet Rescue - Knowledge Base Documentation

## Overview

Pawsitive Haven is a **pet rescue organization** that saves dogs through foster-based rescue. This document catalogs all knowledge base resources available for the AI chatbot to assist fosters, adopters, and volunteers.

---

## Knowledge Base Structure

```
PawsitiveHaven/
├── database/
│   └── init.sql                    # FAQ seed data (26 Q&A pairs)
├── src/PawsitiveHaven.Web/
│   └── wwwroot/assets/documents/   # Guide documents
│       ├── organizational-structure-contacts.md
│       ├── first-time-foster-guide.md
│       ├── adoption-process-guide.md
│       └── foster-care-checklist.md
└── docs/
    ├── CHATBOT_SECURITY.md         # Security documentation
    ├── CHATBOT_IMPLEMENTATION_PLAN.md
    └── KNOWLEDGE_BASE.md           # This file
```

---

## Document Summaries

### 1. Organizational Structure & Contact Directory

**File:** `wwwroot/assets/documents/organizational-structure-contacts.md`
**Size:** ~7KB
**Purpose:** Complete contact directory for the organization

**Topics Covered:**
- Emergency contact numbers (vet, lost dog, foster hotline)
- Leadership team and board of directors
- Department email directory (adoption, foster, vet, transport, marketing)
- Regional foster coordinators
- Specialty teams (puppy, senior, medical, big dog)
- Communication channels (Slack, Facebook, newsletter)
- Office hours and locations
- Quick reference: "Who do I contact?"

**Target Queries:**
- "How do I contact the adoption team?"
- "What's the emergency vet number?"
- "Who is my regional coordinator?"
- "What's the email for supplies?"

---

### 2. First Time Foster Guide

**File:** `wwwroot/assets/documents/first-time-foster-guide.md`
**Size:** ~12KB
**Purpose:** Complete onboarding guide for new foster families

**Topics Covered:**
- Before pickup: preparation checklist
- At pickup: what you'll receive, questions to ask
- First 72 hours: decompression period, the 3-3-3 rule
- Potty training protocol
- Crate training
- Introducing to resident pets (dogs and cats)
- Training basics after quarantine
- Marketing your foster (photos, bio)
- Medical care responsibilities
- When your foster gets adopted
- Taking a break from fostering

**Target Queries:**
- "What is the 3-3-3 rule?"
- "How do I introduce my foster to my dog?"
- "What supplies do I need?"
- "My foster isn't eating, is this normal?"
- "How do I help my foster get adopted?"

---

### 3. Adoption Process Guide

**File:** `wwwroot/assets/documents/adoption-process-guide.md`
**Size:** ~11KB
**Purpose:** Step-by-step guide for potential adopters

**Topics Covered:**
- Adoption philosophy
- 12-step adoption process overview
- Browsing available dogs
- Application requirements and tips
- Application review criteria
- Phone/video interview
- Meet and greet scheduling
- Home check (when required)
- Adoption fees and discounts
- What's included in adoption fee
- Adoption appointment checklist
- Taking your dog home
- Two-week check-in
- Lifetime support
- Foster-to-adopt option
- FAQs for adopters

**Target Queries:**
- "How do I adopt a dog?"
- "What are the adoption fees?"
- "Can I adopt if I rent?"
- "What's included in the adoption fee?"
- "How long does adoption take?"

---

### 4. Foster Care Checklist

**File:** `wwwroot/assets/documents/foster-care-checklist.md`
**Size:** ~8KB
**Purpose:** Printable checklist for tracking foster tasks

**Topics Covered:**
- Before pickup tasks
- At pickup verification
- First 24 hours checklist
- First 72 hours (decompression) daily checklist
- First week tasks (health monitoring, behavioral observations)
- Week 2 tasks (introductions, marketing)
- Weeks 3-4 tasks (socialization, training)
- Monthly tasks (prevention, admin)
- Semi-annual tasks (long-term fosters)
- Adoption transition checklist
- Notes section for tracking

**Target Queries:**
- "What should I do in the first week?"
- "When do I give heartworm prevention?"
- "What do I track for my foster?"
- "How do I prepare for foster pickup?"

---

## FAQ Database

**Location:** `database/init.sql`
**Count:** 26 FAQ entries
**Categories:**

| Category | Count | Topics |
|----------|-------|--------|
| Adoption Process | 5 | Hours, process, fees, renting, timeline |
| Fostering | 5 | How to foster, supplies, duration, 3-3-3, other pets |
| Medical & Care | 5 | Vaccinations, sick dogs, expenses, prevention, decompression |
| Behavior & Training | 5 | House training, introductions, fear, training, behavior support |
| Marketing & Adoption | 3 | Photos, interest, meet and greets |
| Support & Resources | 3 | Lost dog, supplies, taking a break |

---

## Content Categories Mapping

### For Foster Volunteers

| Topic | Primary Document | FAQ Support |
|-------|------------------|-------------|
| Getting started | First Time Foster Guide | FAQs 6-10 |
| Daily care | Foster Care Checklist | FAQs 14-15 |
| Medical needs | First Time Foster Guide (Medical section) | FAQs 11-14 |
| Behavior help | First Time Foster Guide (Training section) | FAQs 16-20 |
| Contacts | Organizational Structure | FAQs 24-26 |

### For Potential Adopters

| Topic | Primary Document | FAQ Support |
|-------|------------------|-------------|
| Adoption process | Adoption Process Guide | FAQs 1-5 |
| Fees and requirements | Adoption Process Guide | FAQs 3-4 |
| Post-adoption | Adoption Process Guide (Steps 10-12) | - |

### For General Inquiries

| Topic | Primary Document | FAQ Support |
|-------|------------------|-------------|
| Contact info | Organizational Structure | All |
| Hours/locations | Organizational Structure | FAQ 1 |
| Volunteering | Organizational Structure | FAQ 6 |

---

## Emergency Information Quick Reference

**For AI to prioritize in emergency-related queries:**

| Emergency | Immediate Action | Contact |
|-----------|------------------|---------|
| Lost foster dog | Call IMMEDIATELY | (555) PAW-LOST |
| Medical emergency | Call or go to ER | (555) PAW-VET1 |
| Bite/injury incident | Report immediately | (555) PAW-SAFE |
| After-hours foster support | Call hotline | (555) PAW-HELP |

---

## AI Context Injection

### Current Implementation

FAQs are injected into the AI system prompt:
```csharp
var faqContext = string.Join("\n", faqs.Select(f =>
    $"Q: {f.Question}\nA: {f.Answer}"));
```

### Recommended Enhancement

Structure FAQ injection with clear delimiters:
```
---FAQ_KNOWLEDGE_START---
Q: [Question]
A: [Answer]
---FAQ_KNOWLEDGE_END---
```

### Future: Document Embedding

For enhanced retrieval, consider:
1. Embed document chunks in vector database
2. Semantic search for relevant content
3. Inject only relevant sections into context
4. Track which documents informed responses

---

## Content Metrics

| Document | Word Count | Est. Tokens |
|----------|------------|-------------|
| Organizational Structure | ~1,800 | ~2,400 |
| First Time Foster Guide | ~3,200 | ~4,200 |
| Adoption Process Guide | ~2,900 | ~3,800 |
| Foster Care Checklist | ~2,100 | ~2,800 |
| **Total Guides** | **~10,000** | **~13,200** |
| FAQ Database (26 entries) | ~3,200 | ~4,200 |
| **Grand Total** | **~13,200** | **~17,400** |

*Note: Token estimates assume ~1.3 tokens per word for English text*

---

## Comparison to Sister Project (AA-CHATBOT)

| AA-CHATBOT (Angels Among Us) | Pawsitive Haven |
|------------------------------|-----------------|
| Real rescue organization | Fictional rescue organization |
| Specific real contacts/emails | Fictional contacts/emails |
| 2024 dated documents | 2026 dated documents |
| Georgia-based regional structure | Generic metro regional structure |
| 4 PDF documents | 4 markdown documents |
| Similar content structure | Same content structure |

**Key differences:**
- All names, emails, and phone numbers are fictional
- Organizational structure simplified for demo purposes
- Content is original while following same format/topics

---

## Adding New Content

### Process for New FAQs

1. Identify common question not covered
2. Research accurate answer
3. Write in consistent Q&A format
4. Add via Admin Dashboard or database insert
5. Test with AI chat to verify retrieval

### Process for New Documents

1. Identify topic gap
2. Create comprehensive markdown document
3. Follow existing document structure
4. Place in `wwwroot/assets/documents/`
5. Update this knowledge base catalog
6. Consider adding related FAQs

---

## Security Considerations

See [CHATBOT_SECURITY.md](./CHATBOT_SECURITY.md) for:
- Prompt injection prevention
- FAQ content sanitization
- Safe knowledge base injection
- Output filtering

---

*Document Version: 1.0 | Last Updated: February 2026*

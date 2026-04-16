UI/UX Sub-Agent Instructions
Visual Identity & Aesthetic
Core Style: Strict "Neumorphism" (Soft UI). [cite_start]Use play of light and shadow instead of harsh borders[cite: 15].
Color Palette: - Primary: #E6E6FA (Lavender Mist)
Secondary: #FFFFFF (Crisp White)
Accent: #9370DB (Medium Purple) for active states and icons.
Layout: Use a side-navigation layout with a floating content area[cite: 6].
Non-Standard Components: Avoid <table> elements at all costs[cite: 304]. [cite_start]Use elevated "Soft-UI Tiles" with box-shadow to represent data[cite: 307].

UX Principles
Design Constraint: Every component must have rounded corners (minimum 25px) to maintain the soft aesthetic[cite: 15].
Interactivity: Implement smooth "press-in" animations for buttons and "lift" transitions for cards on hover[cite: 369].
Navigation: Every page MUST include a breadcrumb trail at the top (e.g., Home > Patients > Details) and a permanent link back to the Dashboard[cite: 14].
Typography: Use a rounded sans-serif font (like 'Quicksand' or 'Nunito') for a friendly, modern veterinary feel[cite: 374].

Implementation Rules
Tag Helpers: Prioritize asp-controller and asp-action for all links to ensure correct routing[cite: 183].
Responsive: Use CSS Grid with repeat(auto-fill, minmax(300px, 1fr)) for entity lists to ensure the cards look good on all screens[cite: 231, 232].
# Colour Tokens

Colour tokens define colour for components and global styles.

## Colour tokens and accessibility

GC Design System components meet level AA of the Web Content Accessibility Guidelines (WCAG 2.1) colour contrast standards for text and interactive elements.  
When you use global tokens, check if your colour combinations meet standards for accessible colour contrast using [WebAIM Contrast Checker](https://webaim.org/resources/contrastchecker/).

## Select colour tokens purposefully

- Select global tokens based on their purpose and its match to your needs.
- Replace hard-coded values in your code with GC Design System colour tokens, like `var(--gcds-text-primary)` instead of `#000000`.

## Global colour tokens

Use global tokens to:

- Design layouts, text colours, and global (or site-wide) styles for typography or spacing.
- Communicate the meaning associated with that token (Global tokens are semantic).
- Create new component tokens.
- Use global state tokens: Apply standard styling for states to your own components.

### Text

| Token name            | Contrast ratio | Hex      | Purpose                                                                                  |
|-----------------------|:--------------:|----------|------------------------------------------------------------------------------------------|
| --gcds-text-light     | 1              | #FFF     | Main light text colour. Use on background shade of 700 or darker (like --gcds-bg-dark).  |
| --gcds-text-primary   | 12.63          | #333333  | Main text colour. Use on a background shade of 50 or lighter (like --gcds-bg-white).     |
| --gcds-text-secondary | 9.33           | #43474E  | Contrast text colour (alternative to primary). Use on background shade of 50 or lighter. |

### Link

| Token name          | Contrast ratio | Hex      | Purpose                                                                              |
|---------------------|:--------------:|----------|--------------------------------------------------------------------------------------|
| --gcds-link-default | 10.38          | #284162  | Default link colour for links on a white background.                                  |
| --gcds-link-hover   | 8.58           | #0535d2  | Hover link colour for links on a white background.                                    |
| --gcds-link-light   | 1              | #FFF     | Main light link colour for links on 700 shade or darker background.                   |
| --gcds-link-visited | 7.22           | #7532B8  |                                                                                      |

### Background

| Token name          | Contrast ratio | Hex      | Purpose                                                                              |
|---------------------|:--------------:|----------|--------------------------------------------------------------------------------------|
| --gcds-bg-dark      | 12.63          | #333333  | Main dark background colour. Use with a text shade of 100 or lighter.                |
| --gcds-bg-light     | 1.12           | #F1F2F3  | Light background colour (alternative to white). Use with text shade of 700 or darker.|
| --gcds-bg-primary   | 12.15          | #26374A  | Highlight background colour. Use with text shade of 100 or lighter.                  |
| --gcds-bg-white     | 1              | #FFF     | Main background colour. Use with text shade of 700 or darker.                        |

### Border

| Token name            | Contrast ratio | Hex      | Purpose                                                        |
|-----------------------|:--------------:|----------|----------------------------------------------------------------|
| --gcds-border-default | 3.86           | #7D828B  | Default border colour for borders/icons on white background.    |

### Danger

| Token name              | Contrast ratio | Hex      | Purpose                                                                   |
|-------------------------|:--------------:|----------|---------------------------------------------------------------------------|
| --gcds-danger-background| 1.27           | #FBDDDDA | Danger background colour for emphasis on destructive/critical feedback.    |
| --gcds-danger-border    | 5.51           | #d3080c  | Danger border colour for emphasis on destructive/critical feedback.        |
| --gcds-danger-text      | 7.06           | #A62A1E  | Danger text colour for emphasis on destructive/critical feedback.          |

### Disabled

| Token name                 | Contrast ratio | Hex      | Purpose                                                        |
|----------------------------|:--------------:|----------|----------------------------------------------------------------|
| --gcds-disabled-background | 1.41           | #D6D9DD  | Disabled background colour. Use sparingly for disabled elements.|
| --gcds-disabled-text       | 7.05           | #545961  | Disabled text colour. Use sparingly for disabled elements.      |

### Focus

| Token name              | Contrast ratio | Hex      | Purpose                                                        |
|-------------------------|:--------------:|----------|----------------------------------------------------------------|
| --gcds-focus-background | 8.58           | #0535d2  | Focus background colour, exclusively for focus.                |
| --gcds-focus-border     | 8.58           | #0535d2  |                                                                |
| --gcds-focus-text       | 1              | #FFF     | Focus text colour, exclusively for focus.                      |


## Base colour tokens

Only use base tokens when you've already checked for global tokens and you need something else for building your own components, tokens, or even images.

Use base design tokens to:

- Fill a gap when no global tokens are available.
- Design a custom visual, drawing from the full colour palette.
- Support your own components.
- Provide a basis to build (and potentially contribute back) your own tokens.

### Grayscale

| Token name                  | Contrast ratio | Hex      |
|-----------------------------|:--------------:|----------|
| --gcds-color-grayscale-0    |                | #FFF     |
| --gcds-color-grayscale-50   | 1.12           | #F1F2F3  |
| --gcds-color-grayscale-100  | 1.41           | #D6D9DD  |
| --gcds-color-grayscale-300  | 2.25           | #A8ADB4  |
| --gcds-color-grayscale-500  | 3.86           | #7D828B  |
| --gcds-color-grayscale-700  | 7.05           | #545961  |
| --gcds-color-grayscale-800  | 9.33           | #43474E  |
| --gcds-color-grayscale-900  | 12.63          | #333333  |
| --gcds-color-grayscale-1000 |                | #000     |

### Blue

| Token name                  | Contrast ratio | Hex      |
|-----------------------------|:--------------:|----------|
| --gcds-color-blue-100       | 1.27           | #D7E5F5  |
| --gcds-color-blue-500       | 3.88           | #6584A6  |
| --gcds-color-blue-650       | 10.38          | #284162  |
| --gcds-color-blue-700       | 9.67           | #33465c  |
| --gcds-color-blue-750       | 4.85           | #1E7B96  |
| --gcds-color-blue-800       | 9.47           | #2b4380  |
| --gcds-color-blue-850       | 8.58           | #0535d2  |
| --gcds-color-blue-900       | 12.15          | #26374A  |

### Red

| Token name                  | Contrast ratio | Hex      |
|-----------------------------|:--------------:|----------|
| --gcds-color-red-100        | 1.27           | #FBDDDA  |
| --gcds-color-red-500        | 5.51           | #d3080c  |
| --gcds-color-red-700        | 7.06           | #A62A1E  |
| --gcds-color-red-900        | 9.63           | #822117  |
| --gcds-color-red-flag       | 4.23           | #eb2d37  |

### Green

| Token name                  | Contrast ratio | Hex      |
|-----------------------------|:--------------:|----------|
| --gcds-color-green-100      | 1.11           | #E6F6EC  |
| --gcds-color-green-500      | 3.39           | #289F58  |
| --gcds-color-green-700      | 7.14           | #03662A  |
| --gcds-color-green-800      | 12.78          | #023b1a  |

### Yellow

| Token name                  | Contrast ratio | Hex      |
|-----------------------------|:--------------:|----------|
| --gcds-color-yellow-100     | 1.16           | #FAEDD1  |
| --gcds-color-yellow-500     | 3.49           | #B3800F  |

> **Note:** Code elements take American spelling for "colour" and "grey".

---

## Help us improve

Have questions or a request? Give feedback on our [contact form](https://design-system.alpha.canada.ca/en/contact/).  
Something's wrong? Raise it through [GitHub](https://github.com/canada-ca/design-system) with an account. You'll have access to the team's direct responses, progress made on your issue, and issues raised by others.

- [Give feedback](https://design-system.alpha.canada.ca/en/contact/)
- [Report an issue on GitHub](https://github.com/canada-ca/design-system/issues)

*(Last updated 2024-05-08)*

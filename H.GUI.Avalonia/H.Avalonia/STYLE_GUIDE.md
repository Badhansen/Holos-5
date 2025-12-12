# Global Style Guide for H.Avalonia

This document describes the global styles and resources available for use throughout the H.Avalonia application.

## Converters (Global Resources)

The following converters are available globally and can be used without importing:

- `{StaticResource BoolToBackgroundConverter}` - Converts boolean to background brush
- `{StaticResource BoolToBorderBrushConverter}` - Converts boolean to border brush
- `{StaticResource BoolToBorderThicknessConverter}` - Converts boolean to border thickness
- `{StaticResource BoolToScaleTransformConverter}` - Converts boolean to scale transform

## Color Resources

### Card Colors
- `{StaticResource CardBackgroundBrush}` - White
- `{StaticResource CardSelectedBackgroundBrush}` - #E3F2FD (Light Blue)
- `{StaticResource CardBorderBrush}` - LightGray
- `{StaticResource CardSelectedBorderBrush}` - DodgerBlue
- `{StaticResource CardHoverBackgroundBrush}` - #F8F9FA (Light Gray)

### Badge Colors
- `{StaticResource BadgeBackgroundBrush}` - DodgerBlue
- `{StaticResource BadgeTextBrush}` - White
- `{StaticResource DeleteButtonBrush}` - #FF6B6B (Red)

## Dimension Resources

### Card Dimensions
- `{StaticResource CardBorderThickness}` - 2px
- `{StaticResource CardSelectedBorderThickness}` - 3px
- `{StaticResource CardCornerRadius}` - 8px
- `{StaticResource CardPadding}` - 12px
- `{StaticResource CardMargin}` - 5px

### Badge Dimensions
- `{StaticResource StepBadgeCornerRadius}` - 4px
- `{StaticResource YearBadgeCornerRadius}` - 12px
- `{StaticResource StepBadgePadding}` - 12,6px
- `{StaticResource YearBadgePadding}` - 8,4px

## Style Classes

### Card Styles
```xml
<!-- Basic card style -->
<Border Classes="card">
    <!-- Content -->
</Border>

<!-- Step card with shadow (for main workflow steps) -->
<Border Classes="step-card">
    <!-- Step content -->
</Border>

<!-- Timeline card with selection state -->
<Border Classes="timeline-card">
    <!-- Timeline card content -->
</Border>
```

### Badge Styles
```xml
<!-- Step badge (rectangular) -->
<Border Classes="step-badge">
    <TextBlock Text="Step 1" Classes="step-badge-text"/>
</Border>

<!-- Year badge (circular) -->
<Border Classes="year-badge">
    <TextBlock Text="{Binding Year}" Classes="year-badge-text"/>
</Border>

<!-- Generic badge text -->
<TextBlock Classes="badge-text"/>
```

### Button Styles
```xml
<!-- Delete button -->
<Button Classes="delete-button">
    <TextBlock Text="Delete"/>
</Button>
```

### Container Styles
```xml
<!-- Timeline container -->
<Border Classes="timeline-container">
    <StackPanel>
        <TextBlock Text="Timeline Header" Classes="timeline-header"/>
        <!-- Timeline content -->
        <TextBlock Text="Hint text" Classes="timeline-hint"/>
    </StackPanel>
</Border>
```

### Header Styles
```xml
<!-- Step header with badge and description -->
<StackPanel Classes="step-header">
    <Border Classes="step-badge">
        <TextBlock Text="Step X" Classes="step-badge-text"/>
    </Border>
    <TextBlock Text="Description" Classes="step-number-description"/>
</StackPanel>
```

## Usage Examples

### Complete Timeline Card
```xml
<Button Background="Transparent" BorderThickness="0" Padding="0" Margin="5"
        Command="{Binding SelectCommand}" CommandParameter="{Binding}">
    <Border Classes="timeline-card">
        <StackPanel Spacing="8">
            <Border Classes="year-badge">
                <TextBlock Text="{Binding Year}" Classes="year-badge-text"/>
            </Border>
            
            <StackPanel Spacing="4">
                <TextBlock Text="Select option" FontSize="11" Foreground="Gray"/>
                <ComboBox ItemsSource="{Binding Options}" SelectedItem="{Binding Selected}"/>
            </StackPanel>
            
            <Button Classes="delete-button" Command="{Binding DeleteCommand}">
                <TextBlock Text="Delete"/>
            </Button>
        </StackPanel>
    </Border>
</Button>
```

### Complete Step Header
```xml
<StackPanel Classes="step-header">
    <Border Classes="step-badge">
        <TextBlock Text="Step 1" Classes="step-badge-text"/>
    </Border>
    <TextBlock Text="Enter information" Classes="step-number-description"/>
</StackPanel>
```

### Complete Timeline Container
```xml
<Border Classes="timeline-container">
    <StackPanel>
        <TextBlock Text="History Timeline" Classes="timeline-header"/>
        
        <ScrollViewer HorizontalScrollBarVisibility="Auto">
            <ItemsControl ItemsSource="{Binding Items}">
                <!-- Timeline cards here -->
            </ItemsControl>
        </ScrollViewer>
        
        <TextBlock Text="Tip: Scroll to view all items" Classes="timeline-hint"/>
    </StackPanel>
</Border>
```

## Selection State Implementation

For interactive cards that need selection state:

1. **Add `IsSelected` property to your DTO**:
```csharp
public bool IsSelected { get; set; }
```

2. **Update selection in your ViewModel**:
```csharp
private void UpdateSelectionStates(MyDto selectedItem)
{
    foreach (var item in Items)
    {
        item.IsSelected = item == selectedItem;
    }
}
```

3. **Use the timeline-card class**:
```xml
<Border Classes="timeline-card">
    <!-- The converters will handle the visual state based on IsSelected -->
</Border>
```

## Benefits

- **Consistency**: All UI components use the same colors, dimensions, and behaviors
- **Maintainability**: Change styles globally by updating the resource files
- **Reusability**: Easy to create new components using existing patterns
- **Performance**: Styles are loaded once and reused throughout the app
- **Theme Support**: Easy to implement different themes by changing global resources

## Adding New Styles

When adding new reusable styles:

1. **Colors/Brushes**: Add to `ResourceStyles.axaml` in the `<Styles.Resources>` section
2. **Style Classes**: Add to `ControlsStyles.axaml` using the pattern `Style Selector="Element.class-name"`
3. **Document**: Update this guide with usage examples
4. **Test**: Ensure the style works across different scenarios

Remember to use semantic naming that describes the purpose, not the appearance (e.g., `BadgeBackgroundBrush` rather than `BlueBrush`).
// dropdown.js - Click outside handler for SearchableDropdown
let clickOutsideHandler = null;
let currentDropdownElement = null;

export function addClickOutsideListener(dropdownElement, dotNetHelper) {
    // Remove previous listener if exists
    removeClickOutsideListener();
    
    currentDropdownElement = dropdownElement;
    
    // Create new click handler
    clickOutsideHandler = function(event) {
        // Check if click is outside the dropdown
        if (dropdownElement && !dropdownElement.contains(event.target)) {
            dotNetHelper.invokeMethodAsync('CloseDropdown');
        }
    };
    
    // Add listener on next tick to avoid immediate trigger
    setTimeout(() => {
        document.addEventListener('click', clickOutsideHandler, true);
    }, 0);
}

export function removeClickOutsideListener() {
    if (clickOutsideHandler) {
        document.removeEventListener('click', clickOutsideHandler, true);
        clickOutsideHandler = null;
        currentDropdownElement = null;
    }
}

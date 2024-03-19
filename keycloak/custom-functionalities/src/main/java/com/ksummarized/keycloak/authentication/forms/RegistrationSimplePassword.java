package com.ksummarized.keycloak.authentication.forms;

import java.util.ArrayList;

import org.keycloak.authentication.forms.RegistrationPage;
import org.keycloak.authentication.forms.RegistrationPassword;
import org.keycloak.authentication.ValidationContext;
import org.keycloak.events.Details;
import org.keycloak.events.Errors;
import org.keycloak.models.utils.FormMessage;
import org.keycloak.policy.PasswordPolicyManagerProvider;
import org.keycloak.policy.PolicyError;
import org.keycloak.services.messages.Messages;
import org.keycloak.services.validation.Validation;

import jakarta.ws.rs.core.MultivaluedMap;
import java.util.List;


public class RegistrationSimplePassword extends RegistrationPassword {
    public static final String PROVIDER_ID = "registration-simple-password-action";

    @Override
    public String getHelpText() {
        return "Validates only the password field. Use it to get rid of the confirm password field from register form. It also will store password in user's credential store.";
    }

    @Override
    public String getDisplayType() {
        return "Password Validation Without Confirm Field";
    }

    /**
     * Gets the ID of the provider. This should be unique across all registered providers.
     *
     * @return the provider ID
     */
    @Override
    public String getId() {
        return PROVIDER_ID;
    }

    @Override
    public void validate(ValidationContext context) {
        MultivaluedMap<String, String> formData = context.getHttpRequest().getDecodedFormParameters();
        List<FormMessage> errors = new ArrayList<>();
        context.getEvent().detail(Details.REGISTER_METHOD, "form");
        if (Validation.isBlank(formData.getFirst(RegistrationPage.FIELD_PASSWORD))) {
            errors.add(new FormMessage(RegistrationPage.FIELD_PASSWORD, Messages.MISSING_PASSWORD));
        }
        if (formData.getFirst(RegistrationPage.FIELD_PASSWORD) != null) {
            PolicyError err = context.getSession().getProvider(PasswordPolicyManagerProvider.class).validate(context.getRealm().isRegistrationEmailAsUsername() ? formData.getFirst(RegistrationPage.FIELD_EMAIL) : formData.getFirst(RegistrationPage.FIELD_USERNAME), formData.getFirst(RegistrationPage.FIELD_PASSWORD));
            if (err != null)
                errors.add(new FormMessage(RegistrationPage.FIELD_PASSWORD, err.getMessage(), err.getParameters()));
        }

        if (!errors.isEmpty()) {
            context.error(Errors.INVALID_REGISTRATION);
            formData.remove(RegistrationPage.FIELD_PASSWORD);
            context.validationError(formData, errors);
        } else {
            context.success();
        }
    }
}

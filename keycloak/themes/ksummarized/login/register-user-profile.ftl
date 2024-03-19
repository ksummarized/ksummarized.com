<#import "template.ftl" as layout>
<#import "user-profile-commons.ftl" as userProfileCommons>
<#import "register-commons.ftl" as registerCommons>
<@layout.registrationLayout displayMessage=messagesPerField.exists('global') displayInfo=true displayRequiredFields=true; section>
    <#if section = "header">
        <img id="ksummarized-logo" src="${url.resourcesPath}/img/KsummarizedLogo.png" alt="ksummarized logo" />
    <#elseif section = "form">
        <form id="kc-register-form" class="${properties.kcFormClass!}" action="${url.registrationAction}" method="post">

            <@userProfileCommons.userProfileFormFields; callback, attribute>
                <#if callback = "afterField">
	                <#-- render password fields just under the username or email (if used as username) -->
		            <#if passwordRequired?? && (attribute.name == 'username' || (attribute.name == 'email' && realm.registrationEmailAsUsername))>
		                <div class="${properties.kcFormGroupClass!}">
		                    <div class="${properties.kcLabelWrapperClass!}">
		                        <label for="password" class="${properties.kcLabelClass!}">${msg("password")}</label>
		                    </div>
		                    <div class="${properties.kcInputWrapperClass!}">
								<div class="${properties.kcInputGroup!}">
									<input type="password" id="password" class="${properties.kcInputClass!}" name="password"
										   autocomplete="new-password"
										   aria-invalid="<#if messagesPerField.existsError('password','password-confirm')>true</#if>"
									/>
									<button class="${properties.kcFormPasswordVisibilityButtonClass!}" type="button" aria-label="${msg('showPassword')}"
											aria-controls="password"  data-password-toggle
											data-icon-show="${properties.kcFormPasswordVisibilityIconShow!}" data-icon-hide="${properties.kcFormPasswordVisibilityIconHide!}"
											data-label-show="${msg('showPassword')}" data-label-hide="${msg('hidePassword')}">
										<i class="${properties.kcFormPasswordVisibilityIconShow!}" aria-hidden="true"></i>
									</button>
								</div>

		                        <#if messagesPerField.existsError('password')>
		                            <span id="input-error-password" class="${properties.kcInputErrorMessageClass!}" aria-live="polite">
		                                ${kcSanitize(messagesPerField.get('password'))?no_esc}
		                            </span>
		                        </#if>
		                    </div>
		                </div>

		            </#if>
                </#if>
            </@userProfileCommons.userProfileFormFields>

            <@registerCommons.termsAcceptance/>

            <#if recaptchaRequired??>
                <div class="form-group">
                    <div class="${properties.kcInputWrapperClass!}">
                        <div class="g-recaptcha" data-size="compact" data-sitekey="${recaptchaSiteKey}"></div>
                    </div>
                </div>
            </#if>

            <div class="${properties.kcFormGroupClass!}">
                <div id="kc-form-buttons" class="${properties.kcFormButtonsClass!}">
                    <input class="${properties.kcButtonClass!} ${properties.kcButtonPrimaryClass!} ${properties.kcButtonBlockClass!} ${properties.kcButtonLargeClass!}" type="submit" value="${msg("doRegister")}"/>
                </div>
            </div>
        </form>
		<script type="module" src="${url.resourcesPath}/js/passwordVisibility.js"></script>
	<#elseif section = "info" >
        <#if realm.password && realm.registrationAllowed && !registrationDisabled??>
            <div id="kc-registration-container">
                <div id="kc-registration">
                    <span>${msg("alreadyRegistered")} <a tabindex="8" class="ks-link"
                                                 href="${url.loginUrl}">${msg("doLogIn")}</a></span>
                </div>
            </div>
        </#if>
	<#elseif section = "socialProviders" >
        <#if realm.password && social.providers??>
            <div id="kc-social-providers" class="${properties.kcFormSocialAccountSectionClass!}">
                <span>${msg("identity-provider-register-label")}</span>

                <ul class="${properties.kcFormSocialAccountListClass!} <#if social.providers?size gt 3>${properties.kcFormSocialAccountListGridClass!}</#if>">
                    <#list social.providers as p>
                        <li>
                            <a id="social-${p.alias}" class="${properties.kcFormSocialAccountListButtonClass!} <#if social.providers?size gt 3>${properties.kcFormSocialAccountGridItem!}</#if>"
                                    type="button" href="${p.loginUrl}">
                                <#switch p.alias>
                                    <#case "google">
                                        <img src="${url.resourcesPath}/img/GoogleLogo.svg" alt="${p.alias}" />
                                        <#break>
                                    <#case "facebook">
                                        <img src="${url.resourcesPath}/img/FacebookLogo.svg" alt="${p.alias}" />
                                        <#break>
                                    <#case "twitter">
                                        <img src="${url.resourcesPath}/img/XLogo.svg" alt="${p.alias}" />
                                        <#break>
                                    <#case "github">
                                        <img src="${url.resourcesPath}/img/GithubLogo.svg" alt="${p.alias}" />
                                        <#break>
                                    <#default>
                                        <span class="${properties.kcFormSocialAccountNameClass!}">${p.displayName!}</span>
                                </#switch>
                            </a>
                        </li>
                    </#list>
                </ul>
                <div class="separator">or</div>
            </div>
        </#if>
    </#if>
</@layout.registrationLayout>

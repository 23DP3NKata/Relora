using System.Net;

namespace Relora.Shared.Application.Emails.Templates;

public static class SupportRequestReceivedEmailTemplate
{
    private const string BrandColor = "#202022";
    private const string TextColor = "#1F2937";
    private const string MutedColor = "#6B7280";
    private const string BorderColor = "#E5E7EB";
    private const string BackgroundColor = "#F3F4F6";

    public static string Create(string subject, string? requestId = null)
    {
        var encodedSubject = WebUtility.HtmlEncode(subject);

        var requestIdRow = string.IsNullOrWhiteSpace(requestId)
            ? string.Empty
            : $"""
                <tr>
                    <td style="padding: 4px 0; color: {MutedColor}; font-size: 14px; width: 140px;">Request ID</td>
                    <td style="padding: 4px 0; color: {TextColor}; font-size: 14px; font-weight: 600;">#{WebUtility.HtmlEncode(requestId)}</td>
                </tr>
                """;

        return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                <title>Support request received</title>
            </head>
            <body style="margin:0; padding:0; background-color:{BackgroundColor}; font-family: -apple-system, Segoe UI, Roboto, Helvetica, Arial, sans-serif;">
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background-color:{BackgroundColor}; padding: 32px 16px;">
                    <tr>
                        <td align="center">
                            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="max-width: 560px; background-color:#FFFFFF; border-radius: 12px; overflow: hidden; border: 1px solid {BorderColor};">
                                <tr>
                                    <td style="background-color:{BrandColor}; padding: 28px 32px;">
                                        <span style="color:#FFFFFF; font-size: 20px; font-weight: 700; letter-spacing: 0.3px;">Relora</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding: 32px;">
                                        <p style="margin:0 0 16px; color:{TextColor}; font-size: 16px; line-height: 1.5;">Hello,</p>
                                        <p style="margin:0 0 24px; color:{TextColor}; font-size: 16px; line-height: 1.5;">
                                            We've received your support request and our team will take a look shortly.
                                        </p>

                                        <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background-color:{BackgroundColor}; border-radius: 8px; padding: 16px 20px; margin-bottom: 24px;">
                                            <tr>
                                                <td style="padding: 4px 0; color:{MutedColor}; font-size:14px; width: 140px;">Subject</td>
                                                <td style="padding: 4px 0; color:{TextColor}; font-size:14px; font-weight:600;">{encodedSubject}</td>
                                            </tr>
                                            {requestIdRow}
                                        </table>

                                        <p style="margin:0 0 24px; color:{TextColor}; font-size: 16px; line-height: 1.5;">
                                            We'll get back to you at this email address as soon as possible.
                                        </p>

                                        <p style="margin:0; color:{TextColor}; font-size: 16px; line-height: 1.5;">
                                            - Relora Support
                                        </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding: 20px 32px; border-top: 1px solid {BorderColor};">
                                        <p style="margin:0; color:{MutedColor}; font-size: 12px; line-height: 1.5;">
                                            This is an automated message, please do not reply directly to this email.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;
    }
}
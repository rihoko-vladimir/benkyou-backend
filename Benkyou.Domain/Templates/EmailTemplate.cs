namespace Benkyou.Domain.Templates;

public class EmailTemplate
{
    private readonly string _emailTemplateString = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <title>Email confirmation</title>
    <link
            rel=""stylesheet""
    href=""https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap""
    />
    <style>
        html{{
            height: 100%;
        }}
        body {{
            width: 100%;
            height: 100%;
            padding: 0;
            margin: 0;
            font-family: ""Roboto"", ""Helvetica"", ""Arial"", sans-serif;
        }}

        .header {{
            background-color: #eef6f6;
            padding: 24px;
            border-top-right-radius: 12px;
            border-bottom-right-radius: 12px;
        }}

        .container {{
            box-sizing: border-box;
            width: 630px;
            display: flex;
            flex-direction: column;
            background-color: white;
            z-index: 4;
            border-radius: 12px;
            margin: 32px;
            box-shadow: 0 14px 28px rgba(0, 0, 0, 0.25), 0 10px 10px rgba(0, 0, 0, 0.22);
        }}

        .content {{
            padding: 24px;
        }}
        .center{{
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            height: 100%;
        }}
    </style>
</head>
<body>
<div class=""center"">
<div class=""container"">
<div class=""header"">
<h2>勉強！</h2>
</div>
<div class=""content"">
<h3>Password reset</h3>
<p>Hello, dear {0}! To reset your password, please, click on the link below:</p>
<br>
<a href=""{1}"">Password reset link</a>
<br>
<br>
<br>
<p>If you haven't asked for password reset, change your password immediately!</p>
</div>
</div>
</div>
</body>
</html>";

    public string GetHtmlPage(string name, string verificationLink)
    {
        return string.Format(_emailTemplateString, name, verificationLink);
    }
}
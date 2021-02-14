# EmailApi
## The Email api is based on the defult VS ASP.NET project
The POST methode accepts JSON that complaies to the next struct:
```C#
public struct Email
        {
            public string subject;
            public string body;
            public string[] distantions;
            public string emailUserAdress;
            public string emailPass;
            public string displayName; //optinal defualt same as emailUserAdress
            public string smtpDomain; //optinal  defualt is Gmail
        }

```

### Exemple POST JSON
```JSON
{
    "subject":"Hello World",
    "body":"<!DOCTYPE html><html><body><h1>הכותרת הראשונה שלי</h1><h1>This is heading 1</h1><h2>This is heading 2</h2><h3>This is heading 3</h3><p>פיסקהl ראשונה</p></body></html>",
    "emailUserAdress":"ExamSenderBot@gmail.com",
    "emailPass":"!Q2w3e4r5t",
    "displayName":"מערכת בדיקה",
    "distantions":["shlomo@neuberger.co.il",""]
}
```


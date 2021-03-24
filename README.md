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
by defult it will send via gmail smtp but it can be changed with adding ```"smtpDomain":"smtp.domain..."``` to the POST data.

### Exemple POST JSON
```JSON
{
    "subject":"Hello World",
    "body":"<!DOCTYPE html><html><body><h1>הכותרת הראשונה שלי</h1><h1>This is heading 1</h1><h2>This is heading 2</h2><h3>This is heading 3</h3><p>פיסקהl ראשונה</p></body></html>",
    "emailUserAdress":"user@domain.com",
    "emailPass":"password",
    "displayName":"מערכת בדיקה",
    "distantions":["user1@domain.co.il","user2@domain.co.il"]
}
```


## Sending file
Adding attachment to the request, should be with your form data.
The important part is that the email arameters specified above will be under the key ```email_parms```

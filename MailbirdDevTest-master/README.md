## The Objective

The objective of this test is to create the back-end code for a small email application. The final result should show:
- Your ability to learn and work with the email component we use, Mail.dll.
- Your attention to detail when designing a UI.
- Your ability to understand the principles of MVVM and use WPF data binding.
- Your ability to create and work with a multi threaded application.
- Your ability to structure your code and avoid code duplication and other code smells.

## Application Requirements

- Connect, using Mail.dll, to a mail server of the type specified in the 'Server type' combobox. The options should be 'IMAP' and 'POP3'. The encryption options should be 'Unencrypted', 'SSL/TLS' and 'STARTTLS'.
- Once connected, the app should begin downloading just the message envelopes/headers for all mail in the inbox, and display the messages in the data grid on the left as they download. The columns should at least include 'From', 'Subject' and 'Date'.
- While downloading the envelopes, the app should also be downloading the message bodies (the actual HTML/Text) of the downloaded envelopes in a separate thread(s), so they're ready to be viewed when a message is clicked.
- Download speed should be the priority, but there should be a hard limit of 5 simultaneous connections to the server at any one time.
- Idle connections should time out and disconnect after one minute of inactivity.
- Clicking on a message in the data grid should select it and show the message body HTML/Text in the text box on the right side. The body should be downloaded from the server on demand if not already downloaded, or if already downloaded just shown immediately.
- In addition, the UI of the app has been purposely built fast, and you are encouraged to improve upon it any way you see fit, using proper containers etc. This is a chance to show off your design and structural skills which we also consider important.

## Mail.dll Reference

The following are direct links to the appropriate samples from the Mail.dll samples page, to help you with the implementation.

### IMAP

- [Connect to server](http://www.limilabs.com/blog/use-ssl-with-imap)
- [Download headers](http://www.limilabs.com/blog/get-email-information-from-imap-fast)
- [Download bodies](http://www.limilabs.com/blog/download-parts-of-email-message)

### POP3

- [Connect to server](http://www.limilabs.com/blog/use-ssl-with-pop3)
- [Download headers](http://www.limilabs.com/blog/get-email-headers-using-pop3-top-command) (you can assume that the TOP command is supported) 
- [Download bodies](http://www.limilabs.com/blog/get-common-email-fields-subject-text-with-pop3)

Note that the included evaluation version of Mail.dll changes the subject of some emails to "Please purchase a license" message and shows "Please purchase a license" dialogs.

## What we look for in particular

- Code duplication is kept to a minimum by using inheritance and otherwise unifying code.
- Magic strings and other code smells are kept to a minimum / is non existent.
- The view (MainWindow.xaml) uses MVVM and data binding extensively/exclusively.
- The solution directories and files are nicely structured.
- The app is built with a focus on speed, using multiple connections to download headers and bodies.
- The app is thread safe and uses proper synchronization.
- Variables have meaningful names, and naming in general, including properties and methods, is consistent with: https://msdn.microsoft.com/en-us/library/ms229002(v=vs.110).aspx


------

It seems Mail.dll doesn't support simultaneous requests with single instance. 

Problems :
1. Downloading email one by one using single instance is working great but sending simultaneous request creating error  "The Write method cannot be called when another write operation is pending". 

2. If emails headers are downloading (one by one) and meanwhile user click on email to body of selected email using same IMap instance from which list of emails are downloading then we are getting error "Tried to read a line. Only '' received. Please make sure that antivirus and firewall software are disabled or configured correctly." also it cause to stop downloading emails by throwing error "Last network operation failed."

Solution :
We can download emails/body simultaneously by creating another instance of IMap but it requires re-connect and login to email server again and again in each download.   So instead Instead of creating seperate theread with 5 concurrent connection I've downloaded emails in sequence using single instance. 
Downloaded body in separate thread by putting hard limit of 5 simultaneous connections at a time.
-
If you are not satiesfied with sequancial downloads then we can change it simultaneous.


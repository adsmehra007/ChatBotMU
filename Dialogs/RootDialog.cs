using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using ChatBotApplication.Models;
using Microsoft.Bot.Builder;
using ChatBot.Controllers;
using ChatBot.Models;
using ChatBotApplication.Helper;
using ChatBotApplication;
using ChatBot.Helper;
using System.Configuration;

namespace ChatBotApplication.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            MessagesController obj = new MessagesController();
            var activity = await result as Activity;
            PatientController pC = new PatientController();
            BotManager bM = new BotManager();
            BOTResponse bR = new BOTResponse();
            var state = string.Empty;
            var input = string.Empty;
            var responseToken = string.Empty;
            string botResponse = string.Empty;
            input = activity.Text.ToString().ToLower();
            bool questionResponded = false;
            //context.From.Name = WebApiApplication.agentName;
            bool isOption = false;
            try
            {
                state = context.ConversationData.GetValue<string>("state");
                responseToken = context.ConversationData.GetValue<string>("ResponseToken");
            }
            catch (Exception ex)
            {
            }
            {
                if (Validator.getOptionSelected(input) > 0 && (state != "firstname" || state != "lastname" || state != "dob" || state != "phone" || state != "ssn" || state != "zip" || state != "reqdescription" || state != "reqdescriptionresp"))
                {
                    var optionNo = Validator.getOptionSelected(input);
                    isOption = true;
                }
                else if (input.Trim() == "1" || input.Trim() == "2" || input.Trim() == "3" || input.Trim() == "4" || input.Trim() == "5" || input.Trim() == "6" || input.Trim() == "7" && (state != "firstname" || state != "lastname" || state != "dob" || state != "phone" || state != "ssn" || state != "zip" || state != "reqdescription" || state != "reqdescriptionresp"))
                {
                    isOption = true;
                }
                else if (input.Contains("appointment") && (state != "firstname" || state != "lastname" || state != "dob" || state != "phone" || state != "ssn" || state != "zip" || state != "reqdescription" || state != "reqdescriptionresp"))
                {
                    input = "1";
                    isOption = true;
                }
                else if (input.Contains("refill") && (state != "firstname" || state != "lastname" || state != "dob" || state != "phone" || state != "ssn" || state != "zip" || state != "reqdescription" || state != "reqdescriptionresp"))
                {
                    input = "2";
                    isOption = true;
                }
                else if (input.Contains("lab") && input.Contains("order") && (state != "firstname" || state != "lastname" || state != "dob" || state != "phone" || state != "ssn" || state != "zip" || state != "reqdescription" || state != "reqdescriptionresp"))
                {
                    input = "3";
                    isOption = true;
                }
                else if (input.Contains("pay") && input.Contains("bill") && (state != "firstname" || state != "lastname" || state != "dob" || state != "phone" || state != "ssn" || state != "zip" || state != "reqdescription" || state != "reqdescriptionresp"))
                {
                    input = "4";
                    isOption = true;
                }
                else if ((input.Contains("contact") || input.Contains("call") || input.Contains("talk")) && input.Contains("doctor") && (state != "firstname" || state != "lastname" || state != "dob" || state != "phone" || state != "ssn" || state != "zip" || state != "reqdescription" || state != "reqdescriptionresp"))
                {
                    input = "5";
                    isOption = true;
                }
                else if (input.Contains("history") && (state != "firstname" || state != "lastname" || state != "dob" || state != "phone" || state != "ssn" || state != "zip" || state != "reqdescription" || state != "reqdescriptionresp"))
                {
                    input = "6";
                    isOption = true;
                }
                else if (input.Contains("something") && (state != "firstname" || state != "lastname" || state != "dob" || state != "phone" || state != "ssn" || state != "zip" || state != "reqdescription" || state != "reqdescriptionresp"))
                {
                    input = "7";
                    isOption = true;
                }
            }
            if (state == "firstname")
            {
                WebApiApplication.firstName = input;
                //bR = pC.SearchPatient(input, 1, null, WebApiApplication.getPatData);
                bR = Validator.BotAPICall("SearchPatient", input, 1, responseToken, WebApiApplication.getPatData);

                if (bR.status == true)
                {
                    context.ConversationData.SetValue<string>("state", "lastname");
                    context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                    context.Activity.From.Name = WebApiApplication.agentName;
                }
                else
                {
                    context.ConversationData.SetValue<string>("state", "firstname");
                }
                await context.PostAsync($"" + bR.ResponseMessage);
                questionResponded = true;
                WebApiApplication.getPatFirstName = bR.FilteredPatList;
            }
            else if (state == "lastname")
            {
                //bR = pC.SearchPatient(input, 2, null, WebApiApplication.getPatFirstName);
                bR = Validator.BotAPICall("SearchPatient", input, 2, responseToken, WebApiApplication.getPatFirstName);
                //if (WebApiApplication.getPatLastName.Count == 1)
                //{
                //    await context.PostAsync($"Thank you for Verifying your Details.");
                //    await context.PostAsync($"How can i help you, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                //    context.ConversationData.SetValue<string>("state", "options");
                //}
                //else
                if (bR.status == true)
                {
                    await context.PostAsync($"" + bR.ResponseMessage);
                    context.ConversationData.SetValue<string>("state", "dob");
                    context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                }
                else
                {
                    await context.PostAsync($"" + bR.ResponseMessage);
                    context.ConversationData.SetValue<string>("state", "");
                }
                questionResponded = true;
                WebApiApplication.getPatLastName = bR.FilteredPatList;
            }
            else if (state == "dob")
            {
                input = ConversionHelper.dateFormat(input);
                if (Validator.checkDateFormat(input))
                {
                    bR = Validator.BotAPICall("SearchPatient", input, 3, responseToken, WebApiApplication.getPatLastName);
                    WebApiApplication.getPatDOB = bR.FilteredPatList;
                    //bR = pC.SearchPatient(input, 3, responseToken, WebApiApplication.getPatLastName);
                    if (bR.status == true)
                    {
                        if (WebApiApplication.getPatDOB.Count == 1)
                        {
                            WebApiApplication.firstName = WebApiApplication.getPatDOB[0].FirstName.Trim();
                            await context.PostAsync($"Thanks for verifying your details " + WebApiApplication.firstName + ". I can now try & " + WebApiApplication.selectedOption + " for you.");
                            context.ConversationData.SetValue<string>("state", "");
                            WebApiApplication.verifiedPat = WebApiApplication.getPatDOB;
                            //await context.PostAsync($"How can i help you, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                            //context.ConversationData.SetValue<string>("state", "options");
                        }
                        else
                        {
                            bool hasSSN = WebApiApplication.getPatDOB.Exists(x => !string.IsNullOrEmpty(x.SSN));
                            bool hasHomePhone = WebApiApplication.getPatDOB.Exists(x => !string.IsNullOrEmpty(x.PatHomePhone));
                            bool hasWorkPhone = WebApiApplication.getPatDOB.Exists(x => !string.IsNullOrEmpty(x.PatWorkPhone));
                            if (hasHomePhone || hasWorkPhone)
                            {
                                context.ConversationData.SetValue<string>("state", "phone");
                                context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                                await context.PostAsync($"Please enter last 4 digits of your Phone number");
                            }
                            else if (hasSSN)
                            {
                                context.ConversationData.SetValue<string>("state", "ssn");
                                context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                                await context.PostAsync($"" + bR.ResponseMessage);
                            }
                            else
                            {

                            }
                        }
                    }
                    else
                    {
                        context.ConversationData.SetValue<string>("state", "dob");
                        await context.PostAsync($"" + bR.ResponseMessage);
                    }
                }
                else
                {
                    context.ConversationData.SetValue<string>("state", "dob");
                    await context.PostAsync($"Please enter valid Date format, (MM/DD/YYYY)");
                }
                questionResponded = true;
                WebApiApplication.getPatDOB = bR.FilteredPatList;
            }
            else if (state == "phone")
            {
                input = Validator.extractNumber(input);
                if (input != "invalid")
                {
                    bR = Validator.BotAPICall("SearchPatient", input, 6, responseToken, WebApiApplication.getPatDOB);
                    WebApiApplication.getPatPhone = bR.FilteredPatList;

                    //bR = pC.SearchPatient(input, 6, responseToken, WebApiApplication.getPatDOB);
                    if (WebApiApplication.getPatPhone.Count == 1)
                    {
                        WebApiApplication.firstName = WebApiApplication.getPatPhone[0].FirstName.Trim();
                        await context.PostAsync($"Thanks for verifying your details " + WebApiApplication.firstName + ". I can now try & " + WebApiApplication.selectedOption + " for you.");
                        context.ConversationData.SetValue<string>("state", "");
                        WebApiApplication.verifiedPat = WebApiApplication.getPatPhone;
                        //await context.PostAsync($"How can i help you, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                        //context.ConversationData.SetValue<string>("state", "options");
                    }
                    else if (bR.status == true)
                    {
                        context.ConversationData.SetValue<string>("state", "ssn");
                        context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                        await context.PostAsync($"" + bR.ResponseMessage);
                    }
                    else
                    {
                        context.ConversationData.SetValue<string>("state", "");
                        var responseMessage = bR.ResponseMessage.Replace("{phone}",input);
                        await context.PostAsync($"" + responseMessage);
                    }
                }
                else
                {
                    context.ConversationData.SetValue<string>("state", "phone");
                    await context.PostAsync($"Sorry doesn't seems like last four digits to me, Please try again (xxxx)");
                }
                questionResponded = true;
                WebApiApplication.getPatPhone = bR.FilteredPatList;
            }
            else if (state == "zip")
            {
                bR = Validator.BotAPICall("SearchPatient", input, 5, responseToken, WebApiApplication.getPatZIP);
                WebApiApplication.getPatZIP = bR.FilteredPatList;
                //bR = pC.SearchPatient(input, 5, responseToken, WebApiApplication.getPatZIP);
                if (WebApiApplication.getPatZIP.Count == 1)
                {
                    WebApiApplication.firstName = WebApiApplication.getPatZIP[0].FirstName.Trim();
                    await context.PostAsync($"Thanks for verifying your details " + WebApiApplication.firstName + ". I can now try & " + WebApiApplication.selectedOption + " for you.");
                    context.ConversationData.SetValue<string>("state", "");
                    WebApiApplication.verifiedPat = WebApiApplication.getPatZIP;
                    //await context.PostAsync($"How can i help you, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                    //context.ConversationData.SetValue<string>("state", "options");
                }
                else if (bR.status == true)
                {
                    context.ConversationData.SetValue<string>("state", "zip");
                    await context.PostAsync($"" + bR.ResponseMessage);
                    //await context.PostAsync($"How can i help you, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                    //context.ConversationData.SetValue<string>("state", "options");
                    context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                }
                else
                {
                    context.ConversationData.SetValue<string>("state", "zip");
                    var responseMessage = bR.ResponseMessage.Replace("{zip}", input);
                    await context.PostAsync($"" + responseMessage);
                }
                questionResponded = true;
            }
            else if (state == "ssn")
            {
                input = Validator.extractNumber(input);
                if (input != "invalid")
                {
                    bR = Validator.BotAPICall("SearchPatient", input, 4, responseToken, WebApiApplication.getPatDOB);
                    WebApiApplication.getPatSSN = bR.FilteredPatList;

                    //bR = pC.SearchPatient(input, 4, responseToken, WebApiApplication.getPatDOB);
                    if (WebApiApplication.getPatSSN.Count == 1)
                    {
                        WebApiApplication.firstName = WebApiApplication.getPatSSN[0].FirstName.Trim();
                        await context.PostAsync($"Thanks for verifying your details " + WebApiApplication.firstName + ". I can now try & " + WebApiApplication.selectedOption + " for you.");
                        context.ConversationData.SetValue<string>("state", "");
                        WebApiApplication.verifiedPat = WebApiApplication.getPatSSN;

                        //await context.PostAsync($"How can i help you, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                        //context.ConversationData.SetValue<string>("state", "options");
                    }
                    else if (bR.status == true)
                    {
                        context.ConversationData.SetValue<string>("state", "zip");
                        context.ConversationData.SetValue<string>("ResponseToken", bR.RequestToken);
                        await context.PostAsync($"" + bR.ResponseMessage);
                    }
                    else
                    {
                        context.ConversationData.SetValue<string>("state", "ssn");
                        var responseMessage = bR.ResponseMessage.Replace("{ssn}", input);
                        await context.PostAsync($"" + responseMessage);
                        //await context.PostAsync($"" + bR.ResponseMessage);
                    }
                }
                else
                {
                    context.ConversationData.SetValue<string>("state", "ssn");
                    await context.PostAsync($"Sorry, this doesn't seems like a valid digits of your SSN, (xxxx)");
                }
                questionResponded = true;
                WebApiApplication.getPatSSN = bR.FilteredPatList;
            }
            else if (state == "reqdescription")
            {
                context.ConversationData.SetValue<string>("state", "reqdescriptionresp");
                await context.PostAsync($"I'm afraid I can't help with this at this time. Can I ask someone to call you?");
                questionResponded = true;
            }
            else if (state == "reqdescriptionresp")
            {
                if (input == "no")
                {
                    context.ConversationData.SetValue<string>("state", "");
                    await context.PostAsync($"My apologies I wasn't of any help.");
                    questionResponded = true;
                }
                else
                {
                    try
                    {
                        if (WebApiApplication.verifiedPat.Count > 0)
                        {
                            var PatHomePhone = WebApiApplication.verifiedPat[0].PatHomePhone;
                            var PatWorkPhone = WebApiApplication.verifiedPat[0].PatWorkPhone;
                            if (!string.IsNullOrWhiteSpace(PatHomePhone))
                            {
                                await context.PostAsync($"" + WebApiApplication.firstName + ", you will shortly receive a call at - " + PatHomePhone);
                            }
                            else if (!string.IsNullOrWhiteSpace(PatWorkPhone))
                            {
                                await context.PostAsync($"" + WebApiApplication.firstName + ", you will shortly receive a call at - " + PatWorkPhone);
                            }
                            else
                            {
                                await context.PostAsync($"We will reach to you soon " + WebApiApplication.firstName + ". Is there anything else i can help you with ?");
                            }
                            context.ConversationData.SetValue<string>("state", "");
                        }
                        else
                        {
                            await context.PostAsync($"I'll need some more details from you so we can contact you at the right number.");
                            context.ConversationData.SetValue<string>("state", "firstname");
                            await context.PostAsync($"Can I know your first name please ?");
                        }
                    }
                    catch (Exception)
                    {
                        await context.PostAsync($"I'll need some more details from you so we can contact you at the right number.");
                        context.ConversationData.SetValue<string>("state", "firstname");
                        await context.PostAsync($"Can I know your first name please ?");
                    }
                    questionResponded = true;
                }
            }
            else if (state == "options" || isOption == true)
            {
                string keyName = "Options.option" + input;
                var keyVal = ConfigurationManager.AppSettings[keyName];
                if (input.Length > 1)
                {
                    input = Validator.getOptionSelected(input).ToString();
                    keyName = "Options.option" + input;
                    try
                    {
                        keyVal = ConfigurationManager.AppSettings[keyName];
                    }
                    catch (Exception)
                    {
                    }
                }
                if (input.ToLower() == "1" || Validator.sentenceComparison(keyVal, input) == true)
                {
                    await context.PostAsync($"Sure, I can help you booking an appointment.");
                    WebApiApplication.selectedOption = "book an appointment";

                    try
                    {
                        if (WebApiApplication.verifiedPat.Count > 0)
                        {
                            await context.PostAsync($"Let us try & " + WebApiApplication.selectedOption + " for you.");
                            context.ConversationData.SetValue<string>("state", "");
                        }
                        else
                        {
                            await context.PostAsync($"Sure, I can help you booking an appointment.");
                            context.ConversationData.SetValue<string>("state", "firstname");
                            await context.PostAsync($"Can I know your first name please ?");
                        }
                    }
                    catch (Exception)
                    {

                        await context.PostAsync($"Sure, I can help you booking an appointment.");
                        context.ConversationData.SetValue<string>("state", "firstname");
                        await context.PostAsync($"Can I know your first name please ?");
                    }

                    questionResponded = true;
                }
                else if (input.ToLower() == "2" || Validator.sentenceComparison(keyVal, input) == true)
                {
                    await context.PostAsync($"Yeah ! Sure i can help you with Request Refills.");
                    WebApiApplication.selectedOption = "request refills";
                    try
                    {
                        if (WebApiApplication.verifiedPat.Count > 0)
                        {
                            await context.PostAsync($"Let us try & " + WebApiApplication.selectedOption + " for you.");
                            context.ConversationData.SetValue<string>("state", "");
                        }
                        else
                        {
                            context.ConversationData.SetValue<string>("state", "firstname");
                            await context.PostAsync($"Can I know your first name please ?");
                        }
                    }
                    catch (Exception)
                    {

                        context.ConversationData.SetValue<string>("state", "firstname");
                        await context.PostAsync($"Can I know your first name please ?");
                    }

                    questionResponded = true;
                }
                else if (input.ToLower() == "3" || Validator.sentenceComparison(keyVal, input) == true)
                {
                    await context.PostAsync($"Sure, I can help you to get an update on Lab Orders.");
                    WebApiApplication.selectedOption = "get an update on lab orders";
                    try
                    {
                        if (WebApiApplication.verifiedPat.Count > 0)
                        {
                            await context.PostAsync($"Let us try & " + WebApiApplication.selectedOption + " for you.");
                            context.ConversationData.SetValue<string>("state", "");
                        }
                        else
                        {
                            context.ConversationData.SetValue<string>("state", "firstname");
                            await context.PostAsync($"Can I know your first name please ?");
                        }
                    }
                    catch (Exception)
                    {
                        context.ConversationData.SetValue<string>("state", "firstname");
                        await context.PostAsync($"Can I know your first name please ?");

                    }

                    questionResponded = true;
                }
                else if (input.ToLower() == "4" || Validator.sentenceComparison(keyVal, input) == true)
                {
                    await context.PostAsync($"Okie dokie, let me help you pay your bills.");
                    WebApiApplication.selectedOption = "get an update on lab orders";
                    try
                    {
                        if (WebApiApplication.verifiedPat.Count > 0)
                        {
                            await context.PostAsync($"Let us try & " + WebApiApplication.selectedOption + " for you.");
                            context.ConversationData.SetValue<string>("state", "");
                        }
                        else
                        {
                            context.ConversationData.SetValue<string>("state", "firstname");
                            await context.PostAsync($"Can I please know your first name ?");
                        }
                    }
                    catch (Exception)
                    {

                        context.ConversationData.SetValue<string>("state", "firstname");
                        await context.PostAsync($"Can I please know your first name ?");
                    }

                    questionResponded = true;
                }
                else if (input.ToLower() == "5" || Validator.sentenceComparison(keyVal, input) == true)
                {
                    await context.PostAsync($"So, you want to contact the Doctor.");
                    WebApiApplication.selectedOption = "contact the doctor";
                    try
                    {
                        if (WebApiApplication.verifiedPat.Count > 0)
                        {
                            await context.PostAsync($"Let us try & " + WebApiApplication.selectedOption + " for you.");
                            context.ConversationData.SetValue<string>("state", "");
                        }
                        else
                        {
                            context.ConversationData.SetValue<string>("state", "firstname");
                            await context.PostAsync($"Can I know your first name please ?");
                        }
                    }
                    catch (Exception)
                    {

                        context.ConversationData.SetValue<string>("state", "firstname");
                        await context.PostAsync($"Can I know your first name please ?");
                    }

                    questionResponded = true;
                }
                else if (input.ToLower() == "6" || Validator.sentenceComparison(keyVal, input) == true)
                {
                    await context.PostAsync($"Sure,i can help you to know about your Medical History.");
                    WebApiApplication.selectedOption = "get your medical history";
                    try
                    {
                        if (WebApiApplication.verifiedPat.Count > 0)
                        {
                            await context.PostAsync($"Let us try & " + WebApiApplication.selectedOption + " for you.");
                            context.ConversationData.SetValue<string>("state", "");
                        }
                        else
                        {
                            context.ConversationData.SetValue<string>("state", "firstname");
                            await context.PostAsync($"Can I know your first name please ?");
                        }
                    }
                    catch (Exception)
                    {

                        context.ConversationData.SetValue<string>("state", "firstname");
                        await context.PostAsync($"Can I know your first name please ?");
                    }

                    questionResponded = true;
                }
                else if (input.ToLower() == "7" || Validator.sentenceComparison(keyVal, input) == true)
                {
                    await context.PostAsync($"Please type a brief description of your requirement.");
                    context.ConversationData.SetValue<string>("state", "reqdescription");
                    questionResponded = true;
                }
                else
                {
                    //await context.PostAsync($"Sorry, Cannot process this input at the moment.");
                    try
                    {
                        var commonQuestions = bM.getCommonQuestions();
                        foreach (var cQuestion in commonQuestions)
                        {
                            try
                            {
                                bool isMatch = Validator.sentenceComparison(cQuestion.Question, input);
                                if (isMatch == true)
                                {
                                    if (cQuestion.ConversationStateValue == "initgreet")
                                    {
                                        await context.PostAsync($"How can i help you today ? {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                                        context.ConversationData.SetValue<string>("state", "options");
                                    }
                                    else if (cQuestion.ConversationStateValue == "greet")
                                    {
                                        botResponse = cQuestion.Answer;
                                        botResponse = botResponse.Replace("{greet}", Validator.checkDayGreeting());
                                        await context.PostAsync(botResponse);
                                        context.ConversationData.SetValue<string>(cQuestion.ConversationState.ToLower(), cQuestion.ConversationStateValue.ToLower());
                                    }
                                    else if (cQuestion.ConversationStateValue == "optionlist")
                                    {
                                        await context.PostAsync($"I can help you with following things  {Environment.NewLine}{Environment.NewLine} 1.Book an appointment{Environment.NewLine} 2.Request Refills{Environment.NewLine} 3.Update on Lab Orders{Environment.NewLine} 4.Pay your bills{Environment.NewLine} 5.Contact the Doctor{Environment.NewLine} 6.Inquiries about your Medical history{Environment.NewLine}  7.Something else{Environment.NewLine}{Environment.NewLine}How can i help you today?");
                                        context.ConversationData.SetValue<string>("state", "options");
                                    }
                                    else
                                    {
                                        botResponse = cQuestion.Answer;
                                        await context.PostAsync(botResponse);
                                        context.ConversationData.SetValue<string>(cQuestion.ConversationState.ToLower(), cQuestion.ConversationStateValue.ToLower());
                                    }
                                    questionResponded = true;
                                    break;
                                }
                                else
                                {
                                    questionResponded = false;
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            else
            {
                try
                {
                    var commonQuestions = bM.getCommonQuestions();
                    foreach (var cQuestion in commonQuestions)
                    {
                        try
                        {
                            bool isMatch = Validator.sentenceComparison(cQuestion.Question, input);
                            if (isMatch == true)
                            {
                                if (cQuestion.ConversationStateValue == "initgreet")
                                {
                                    await context.PostAsync($"How can i help you today ?, {Environment.NewLine}{Environment.NewLine}a)Book an appointment{Environment.NewLine}b)Patient Visit History{Environment.NewLine}c)Pay Bills{Environment.NewLine}d)Refill a request{Environment.NewLine}{Environment.NewLine}Please select an Option");
                                    context.ConversationData.SetValue<string>("state", "options");

                                }
                                else if (cQuestion.ConversationStateValue == "greet")
                                {
                                    botResponse = cQuestion.Answer;
                                    botResponse = botResponse.Replace("{greet}", Validator.checkDayGreeting());
                                    await context.PostAsync(botResponse);
                                    context.ConversationData.SetValue<string>(cQuestion.ConversationState.ToLower(), cQuestion.ConversationStateValue.ToLower());
                                }
                                else if (cQuestion.ConversationStateValue == "optionlist")
                                {
                                    await context.PostAsync($"I can help you with following things  {Environment.NewLine}{Environment.NewLine} 1.Book an appointment{Environment.NewLine} 2.Request Refills{Environment.NewLine} 3.Update on Lab Orders{Environment.NewLine} 4.Pay your bills{Environment.NewLine} 5.Contact the Doctor{Environment.NewLine} 6.Inquiries about your Medical history{Environment.NewLine}  7.Something else{Environment.NewLine}{Environment.NewLine}How can i help you today?");
                                    context.ConversationData.SetValue<string>("state", "options");
                                }
                                else
                                {
                                    botResponse = cQuestion.Answer;
                                    await context.PostAsync(botResponse);
                                    context.ConversationData.SetValue<string>(cQuestion.ConversationState.ToLower(), cQuestion.ConversationStateValue.ToLower());
                                }
                                questionResponded = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            if (questionResponded == false)
            {
                await context.PostAsync($"Sorry, I couldn't get you.");
            }
            //if (input.Contains("hey") || input.Contains("hello"))
            //{
            //    await context.PostAsync($"What is your name ?");
            //    context.ConversationData.SetValue<string>("State","name");
            //}
            //else if (ShortName == null)
            //{
            //    await context.PostAsync($"The ICDcodes is Invalid..Please Enter Valid ICDcode ");
            //}
            //else
            //{
            //    await context.PostAsync($"The ICDcodes is  {activity.Text} and its shortname is  {ShortName} ");
            //    await context.PostAsync($"Do you want to search something else ...press yes or no!!");
            //}
            // Return our reply to the user
            context.Wait(MessageReceivedAsync);
        }

    }
}

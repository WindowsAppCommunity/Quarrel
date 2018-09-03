using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.Storage.Streams;

namespace Discord_UWP.Managers
{
    public class ContactManager
    {
        static ContactStore store;
        static ContactAnnotationStore annotationStore;

        private static async Task<ContactList> GetContactList()
        {
            store = await Windows.ApplicationModel.Contacts.ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);

            if (store == null)
            {
                return null;
            }

            ContactList contactList;

            IReadOnlyList<ContactList> contactLists = await store.FindContactListsAsync();

            if (contactLists.Count == 0)
            {
                contactList = await store.CreateContactListAsync("Discord");
            }
            else
            {
                contactList = contactLists[0];
            }
            return contactList;
        }

        private static async Task<ContactAnnotationList> GetContactAnnotationList()
        {
            annotationStore = await Windows.ApplicationModel.Contacts.ContactManager.RequestAnnotationStoreAsync(ContactAnnotationStoreAccessType.AppAnnotationsReadWrite);

            if (annotationStore == null)
            {
                return null;
            }

            ContactAnnotationList annotationList;

            IReadOnlyList<ContactAnnotationList> annotationLists = await annotationStore.FindAnnotationListsAsync();

            if (annotationLists.Count == 0)
            {
                annotationList = await annotationStore.CreateAnnotationListAsync();
            }
            else
            {
                annotationList = annotationLists[0];
            }

            return annotationList;
        }

        public static async Task<Contact> GetContact(string id)
        {
            store = await Windows.ApplicationModel.Contacts.ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);

            if (store == null)
            {
                return null;
            }

            ContactList contactList;

            IReadOnlyList<ContactList> contactLists = await store.FindContactListsAsync();

            if (contactLists.Count == 0)
            {
                contactList = await store.CreateContactListAsync("Discord");
            }
            else
            {
                contactList = contactLists[0];
            }

            return await contactList.GetContactFromRemoteIdAsync(id);
        }

        private static async Task<bool> CheckContact(SharedModels.User user)
        {
            store = await Windows.ApplicationModel.Contacts.ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);

            if (store == null)
            {
                return true;
            }

            ContactList contactList;

            IReadOnlyList<ContactList> contactLists = await store.FindContactListsAsync();

            if (contactLists.Count == 0)
            {
                contactList = await store.CreateContactListAsync("Discord");
            }
            else
            {
                contactList = contactLists[0];
            }
            var returnval = await contactList.GetContactFromRemoteIdAsync(user.Id);
            return returnval != null;
        }

        private async void CreateTestContacts()
        {
            Contact contact1 = new Contact();
            contact1.FirstName = "TestContact1";

            ContactEmail email1 = new ContactEmail();
            email1.Address = "TestContact1@contoso.com";
            contact1.Emails.Add(email1);

            ContactPhone phone1 = new ContactPhone();
            phone1.Number = "4255550100";
            contact1.Phones.Add(phone1);

            Contact contact2 = new Contact();
            contact2.FirstName = "TestContact2";

            ContactEmail email2 = new ContactEmail();
            email2.Address = "TestContact2@contoso.com";
            email2.Kind = ContactEmailKind.Other;
            contact2.Emails.Add(email2);

            ContactPhone phone2 = new ContactPhone();
            phone2.Number = "4255550101";
            phone2.Kind = ContactPhoneKind.Mobile;
            contact2.Phones.Add(phone2);

            // Save the contacts
            ContactList contactList = await GetContactList();

            if (null == contactList)
            {
                return;
            }

            await contactList.SaveContactAsync(contact1);
            await contactList.SaveContactAsync(contact2);

            //
            // Create annotations for those test contacts.
            // Annotation is the contact meta data that allows People App to generate deep links
            // in the contact card that takes the user back into this app.
            //

            ContactAnnotationList annotationList = await GetContactAnnotationList();

            if (null == annotationList)
            {
                return;
            }

            ContactAnnotation annotation = new ContactAnnotation();
            annotation.ContactId = contact1.Id;

            // Remote ID: The identifier of the user relevant for this app. When this app is
            // launched into from the People App, this id will be provided as context on which user
            // the operation (e.g. ContactProfile) is for.
            annotation.RemoteId = "user12";

            // The supported operations flags indicate that this app can fulfill these operations
            // for this contact. These flags are read by apps such as the People App to create deep
            // links back into this app. This app must also be registered for the relevant
            // protocols in the Package.appxmanifest (in this case, ms-contact-profile).
            annotation.SupportedOperations = ContactAnnotationOperations.ContactProfile;

            if (!await annotationList.TrySaveAnnotationAsync(annotation))
            {
                return;
            }

            annotation = new ContactAnnotation();
            annotation.ContactId = contact2.Id;
            annotation.RemoteId = "user22";

            // You can also specify multiple supported operations for a contact in a single
            // annotation. In this case, this annotation indicates that the user can be
            // communicated via VOIP call, Video Call, or IM via this application.
            annotation.SupportedOperations = ContactAnnotationOperations.ContactProfile | ContactAnnotationOperations.Message;

            if (!await annotationList.TrySaveAnnotationAsync(annotation))
            {
                return;
            }

        }

        public static async void AddContact(SharedModels.User user)
        {
            if (!await CheckContact(user))
            {
                Contact contact = new Contact();
                contact.Name = user.Username + "#" + user.Discriminator;
                contact.RemoteId = user.Id;

                contact.SourceDisplayPicture = RandomAccessStreamReference.CreateFromUri(Common.AvatarUri(user.Avatar, user.Id));

                //ContactEmail email1 = new ContactEmail();
                //email1.Address = "TestContact1@contoso.com";
                //contact1.Emails.Add(email1);

                //ContactPhone phone1 = new ContactPhone();
                //phone1.Number = "4255550100";
                //contact1.Phones.Add(phone1);

                // Save the contacts
                ContactList contactList = await GetContactList();

                if (null == contactList)
                {
                    return;
                }

                try
                {
                    await contactList.SaveContactAsync(contact);
                }
                catch
                {

                }

                //
                // Create annotations for those test contacts.
                // Annotation is the contact meta data that allows People App to generate deep links
                // in the contact card that takes the user back into this app.
                //

                ContactAnnotationList annotationList = await GetContactAnnotationList();

                if (null == annotationList)
                {
                    return;
                }

                ContactAnnotation annotation = new ContactAnnotation();
                annotation.ContactId = contact.Id;

                // Remote ID: The identifier of the user relevant for this app. When this app is
                // launched into from the People App, this id will be provided as context on which user
                // the operation (e.g. ContactProfile) is for.
                annotation.RemoteId = user.Id;

                // The supported operations flags indicate that this app can fulfill these operations
                // for this contact. These flags are read by apps such as the People App to create deep
                // links back into this app. This app must also be registered for the relevant
                // protocols in the Package.appxmanifest (in this case, ms-contact-profile).
                annotation.SupportedOperations = ContactAnnotationOperations.ContactProfile | ContactAnnotationOperations.Message | ContactAnnotationOperations.Share;

                annotation.ProviderProperties.Add("ContactPanelAppID", Windows.ApplicationModel.Package.Current.Id.FamilyName + "!App");

                bool save = await annotationList.TrySaveAnnotationAsync(annotation);
                Debug.WriteLine("saved contact " + user.Username + " save");
            }
        }

        public static async void AddContact(SharedModels.Friend user)
        {
            Contact contact = new Contact();
            contact.FirstName = user.user.Username;
            contact.SourceDisplayPicture = RandomAccessStreamReference.CreateFromUri(Common.AvatarUri(user.user.Avatar, user.Id));
            //ContactEmail email1 = new ContactEmail();
            //email1.Address = "TestContact1@contoso.com";
            //contact1.Emails.Add(email1);

            //ContactPhone phone1 = new ContactPhone();
            //phone1.Number = "4255550100";
            //contact1.Phones.Add(phone1);

            // Save the contacts
            ContactList contactList = await GetContactList();

            if (contactList == null)
            {
                return;
            }

            await contactList.SaveContactAsync(contact);

            //
            // Create annotations for those test contacts.
            // Annotation is the contact meta data that allows People App to generate deep links
            // in the contact card that takes the user back into this app.
            //

            ContactAnnotationList annotationList = await GetContactAnnotationList();

            if (annotationList == null)
            {
                return;
            }

            ContactAnnotation annotation = new ContactAnnotation();
            annotation.ContactId = contact.Id;

            // Remote ID: The identifier of the user relevant for this app. When this app is
            // launched into from the People App, this id will be provided as context on which user
            // the operation (e.g. ContactProfile) is for.
            annotation.RemoteId = user.Id;

            // The supported operations flags indicate that this app can fulfill these operations
            // for this contact. These flags are read by apps such as the People App to create deep
            // links back into this app. This app must also be registered for the relevant
            // protocols in the Package.appxmanifest (in this case, ms-contact-profile).
            annotation.SupportedOperations = ContactAnnotationOperations.ContactProfile | ContactAnnotationOperations.Message | ContactAnnotationOperations.Share;

            if (!await annotationList.TrySaveAnnotationAsync(annotation))
            {
                return;
            }
        }
    }
}

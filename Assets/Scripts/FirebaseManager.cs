using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Threading.Tasks;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;
    
    [Header("Firebase Status")]
    public bool isFirebaseInitialized = false;
    
    // Firebase references
    private FirebaseApp app;
    private FirebaseAuth auth;
    private FirebaseFirestore db;
    
    // Events for UI updates
    public static event Action<bool> OnAuthenticationResult;
    public static event Action<string> OnAuthenticationError;
    public static event Action<bool> OnRegistrationResult;
    public static event Action<string> OnRegistrationError;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFirebase();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private async void InitializeFirebase()
    {
        try
        {
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            
            if (dependencyStatus == DependencyStatus.Available)
            {
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                db = FirebaseFirestore.DefaultInstance;
                
                isFirebaseInitialized = true;
                Debug.Log("✅ Firebase initialized successfully");
            }
            else
            {
                Debug.LogError($"❌ Firebase initialization failed: {dependencyStatus}");
                isFirebaseInitialized = false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Firebase initialization error: {e.Message}");
            isFirebaseInitialized = false;
        }
    }
    
    // Login user with email and password
    public async Task<bool> LoginUser(string email, string password)
    {
        if (!isFirebaseInitialized)
        {
            OnAuthenticationError?.Invoke("Firebase not initialized");
            return false;
        }
        
        try
        {
            var authResult = await auth.SignInWithEmailAndPasswordAsync(email, password);
            var user = authResult.User;
            
            if (user != null)
            {
                Debug.Log($"✅ User logged in: {user.Email}");
                
                // Store user data in Firestore
                await StoreUserLoginData(user.UserId, user.Email);
                
                OnAuthenticationResult?.Invoke(true);
                return true;
            }
            else
            {
                OnAuthenticationError?.Invoke("Login failed - Invalid credentials");
                return false;
            }
        }
        catch (FirebaseException e)
        {
            string errorMessage = GetFirebaseErrorMessage(e);
            OnAuthenticationError?.Invoke(errorMessage);
            Debug.LogError($"❌ Login error: {errorMessage}");
            return false;
        }
    }
    
    // Register new user
    public async Task<bool> RegisterUser(string email, string password)
    {
        if (!isFirebaseInitialized)
        {
            OnRegistrationError?.Invoke("Firebase not initialized");
            return false;
        }
        
        try
        {
            var authResult = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            var user = authResult.User;
            
            if (user != null)
            {
                Debug.Log($"✅ User registered: {user.Email}");
                
                // Create user profile in Firestore
                await CreateUserProfile(user.UserId, user.Email);
                
                // Sign out after registration (user needs to login)
                auth.SignOut();
                
                OnRegistrationResult?.Invoke(true);
                return true;
            }
            else
            {
                OnRegistrationError?.Invoke("Registration failed");
                return false;
            }
        }
        catch (FirebaseException e)
        {
            string errorMessage = GetFirebaseErrorMessage(e);
            OnRegistrationError?.Invoke(errorMessage);
            Debug.LogError($"❌ Registration error: {errorMessage}");
            return false;
        }
    }
    
    // Create user profile in Firestore
    private async Task CreateUserProfile(string userId, string email)
    {
        try
        {
            var userRef = db.Collection("users").Document(userId);
            var userData = new
            {
                email = email,
                createdAt = Timestamp.GetCurrentTimestamp(),
                lastLogin = Timestamp.GetCurrentTimestamp(),
                gamesPlayed = 0,
                highScore = 0
            };
            
            await userRef.SetAsync(userData);
            Debug.Log($"✅ User profile created for: {email}");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error creating user profile: {e.Message}");
        }
    }
    
    // Store user login data
    private async Task StoreUserLoginData(string userId, string email)
    {
        try
        {
            var userRef = db.Collection("users").Document(userId);
            var updateData = new
            {
                lastLogin = Timestamp.GetCurrentTimestamp()
            };
            
            await userRef.UpdateAsync("lastLogin", Timestamp.GetCurrentTimestamp());
            Debug.Log($"✅ Login data updated for: {email}");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error updating login data: {e.Message}");
        }
    }
    
    // Get current user
    public FirebaseUser GetCurrentUser()
    {
        return auth?.CurrentUser;
    }
    
    // Sign out user
    public void SignOut()
    {
        if (auth != null)
        {
            auth.SignOut();
            Debug.Log("✅ User signed out");
        }
    }
    
    // Convert Firebase errors to user-friendly messages
    private string GetFirebaseErrorMessage(FirebaseException e)
    {
        switch (e.ErrorCode)
        {
            case (int)AuthError.InvalidEmail:
                return "Invalid email address";
            case (int)AuthError.WrongPassword:
                return "Incorrect password";
            case (int)AuthError.UserNotFound:
                return "No account found with this email";
            case (int)AuthError.EmailAlreadyInUse:
                return "Email already registered";
            case (int)AuthError.WeakPassword:
                return "Password is too weak";
            case (int)AuthError.NetworkRequestFailed:
                return "Network error. Please check your connection";
            default:
                return $"Authentication error: {e.Message}";
        }
    }
}
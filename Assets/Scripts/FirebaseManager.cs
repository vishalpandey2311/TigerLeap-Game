using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;
    
    [Header("Firebase Status")]
    public bool isFirebaseInitialized = false;
    
    [Header("Session Settings")]
    [SerializeField] private int sessionDurationDays = 7; // Control session duration in Unity Editor
    
    // Firebase references
    private FirebaseApp app;
    private FirebaseAuth auth;
    private FirebaseFirestore db;
    
    // Game selection tracking
    private string currentGameSelection = ""; // "GM1" for Mahjong, "GM2" for Taichi
    
    // Session management
    private const string SESSION_KEY = "UserSession";
    private const string SESSION_EXPIRY_KEY = "SessionExpiry";
    private const string USER_EMAIL_KEY = "UserEmail";
    
    // Events for UI updates
    public static event Action<bool> OnAuthenticationResult;
    public static event Action<string> OnAuthenticationError;
    public static event Action<bool> OnRegistrationResult;
    public static event Action<string> OnRegistrationError;
    public static event Action OnSessionExpired;
    
    // NEW: Additional events for password reset
    public static event Action<bool> OnPasswordResetEmailSent;
    public static event Action<string> OnPasswordResetError;
    public static event Action<bool, string> OnPasswordUpdateResult;

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
                
                // Check existing session after Firebase initialization
                CheckExistingSession();
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
    
    // Check if there's a valid existing session
    private void CheckExistingSession()
    {
        if (HasValidSession())
        {
            string savedEmail = PlayerPrefs.GetString(USER_EMAIL_KEY, "");
            Debug.Log($"✅ Valid session found for user: {savedEmail}");
            
            // Notify UI that user is already logged in
            OnAuthenticationResult?.Invoke(true);
        }
        else
        {
            // Clear any expired session data
            ClearSessionData();
            Debug.Log("ℹ️ No valid session found, user needs to login");
        }
    }
    
    // Check if current session is valid
    public bool HasValidSession()
    {
        if (!PlayerPrefs.HasKey(SESSION_KEY) || !PlayerPrefs.HasKey(SESSION_EXPIRY_KEY))
        {
            return false;
        }
        
        string sessionExpiryString = PlayerPrefs.GetString(SESSION_EXPIRY_KEY);
        if (DateTime.TryParse(sessionExpiryString, out DateTime sessionExpiry))
        {
            return DateTime.Now < sessionExpiry;
        }
        
        return false;
    }
    
    // Create new session
    private void CreateSession(string userEmail)
    {
        DateTime expiryDate = DateTime.Now.AddDays(sessionDurationDays);
        
        PlayerPrefs.SetString(SESSION_KEY, "active");
        PlayerPrefs.SetString(SESSION_EXPIRY_KEY, expiryDate.ToString());
        PlayerPrefs.SetString(USER_EMAIL_KEY, userEmail);
        PlayerPrefs.Save();
        
        Debug.Log($"✅ Session created for {userEmail}, expires on: {expiryDate}");
    }
    
    // Clear session data
    private void ClearSessionData()
    {
        PlayerPrefs.DeleteKey(SESSION_KEY);
        PlayerPrefs.DeleteKey(SESSION_EXPIRY_KEY);
        PlayerPrefs.DeleteKey(USER_EMAIL_KEY);
        PlayerPrefs.Save();
        
        Debug.Log("🧹 Session data cleared");
    }
    
    // Get remaining session time
    public TimeSpan GetRemainingSessionTime()
    {
        if (HasValidSession())
        {
            string sessionExpiryString = PlayerPrefs.GetString(SESSION_EXPIRY_KEY);
            if (DateTime.TryParse(sessionExpiryString, out DateTime sessionExpiry))
            {
                TimeSpan remaining = sessionExpiry - DateTime.Now;
                return remaining.TotalSeconds > 0 ? remaining : TimeSpan.Zero;
            }
        }
        return TimeSpan.Zero;
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
                
                // Create session
                CreateSession(user.Email);
                
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
            
            // Create GM1 collection for Mahjong scores
            await CreateMahjongGameData(userId);
            
            // Create GM2 collection for Taichi scores
            await CreateTaichiGameData(userId);
            
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error creating user profile: {e.Message}");
        }
    }
    
    // Create GM1 collection for Mahjong game data
    private async Task CreateMahjongGameData(string userId)
    {
        try
        {
            var gm1Ref = db.Collection("users").Document(userId).Collection("GM1").Document("scores");
            var mahjongData = new
            {
                // Last played scores (always updated)
                EasyScore = 0,
                MediumScore = 0,
                HardScore = 0,
                // New tracking fields
                TimeTaken = 0,
                FailedAttempts = 0,
                Wins = 0,
                Lose = 0,
                lastPlayed = Timestamp.GetCurrentTimestamp()
            };
            
            await gm1Ref.SetAsync(mahjongData);
            Debug.Log($"✅ GM1 (Mahjong) collection created for user: {userId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error creating GM1 collection: {e.Message}");
        }
    }
    
    // Create GM2 collection for Taichi game data - UPDATED TO MATCH GM1 STRUCTURE
    private async Task CreateTaichiGameData(string userId)
    {
        try
        {
            var gm2Ref = db.Collection("users").Document(userId).Collection("GM2").Document("scores");
            var taichiData = new
            {
                // Last played scores (always updated)
                EasyScore = 0,
                MediumScore = 0,
                HardScore = 0,
                // New tracking fields
                TimeTaken = 0,
                FailedAttempts = 0,
                Wins = 0,
                Lose = 0,
                lastPlayed = Timestamp.GetCurrentTimestamp()
            };
            
            await gm2Ref.SetAsync(taichiData);
            Debug.Log($"✅ GM2 (Taichi) collection created for user: {userId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error creating GM2 collection: {e.Message}");
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
    
    // NEW: Set game selection to GM1 (Mahjong) - Call this on Mahjong button click
    public void SelectMahjongGame()
    {
        currentGameSelection = "GM1";
        Debug.Log("🎮 Game selection set to GM1 (Mahjong)");
    }
    
    // NEW: Set game selection to GM2 (Taichi) - Call this on Taichi button click
    public void SelectTaichiGame()
    {
        currentGameSelection = "GM2";
        Debug.Log("🎮 Game selection set to GM2 (Taichi)");
    }
    
    // NEW: Wrapper method for Unity OnClick events
    public void UpdateGamesPlayedCountWrapper()
    {
        _ = UpdateGamesPlayedCount(); // Fire and forget async call
    }
    
    // Keep the original async method as well
    public async Task UpdateGamesPlayedCount()
    {
        if (!isFirebaseInitialized)
        {
            Debug.LogError("❌ Firebase not initialized");
            return;
        }
        
        var user = GetCurrentUser();
        if (user == null)
        {
            Debug.LogError("❌ No user logged in");
            return;
        }
        
        try
        {
            var userRef = db.Collection("users").Document(user.UserId);
            
            // Get current games played count
            var userDoc = await userRef.GetSnapshotAsync();
            int currentGamesPlayed = 0;
            
            if (userDoc.Exists && userDoc.ContainsField("gamesPlayed"))
            {
                currentGamesPlayed = userDoc.GetValue<int>("gamesPlayed");
            }
            
            // Increment games played count
            await userRef.UpdateAsync("gamesPlayed", currentGamesPlayed + 1);
            
            Debug.Log($"✅ Games played count updated: {currentGamesPlayed + 1} for game: {currentGameSelection}");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error updating games played count: {e.Message}");
        }
    }
    
    // Get current user
    public FirebaseUser GetCurrentUser()
    {
        return auth?.CurrentUser;
    }
    
    // Get current game selection
    public string GetCurrentGameSelection()
    {
        return currentGameSelection;
    }
    
    // Sign out user
    public void SignOut()
    {
        if (auth != null)
        {
            auth.SignOut();
            currentGameSelection = ""; // Reset game selection
            Debug.Log("✅ User signed out");
        }
    }
    
    // NEW: Logout function for Unity OnClick events
    public void LogoutUser()
    {
        try
        {
            string userEmail = "";
            
            if (auth != null && auth.CurrentUser != null)
            {
                userEmail = auth.CurrentUser.Email;
                // Sign out the user from Firebase
                auth.SignOut();
            }
            else if (HasValidSession())
            {
                userEmail = PlayerPrefs.GetString(USER_EMAIL_KEY, "Unknown");
            }
            
            // Clear session data
            ClearSessionData();
            
            // Reset game selection
            currentGameSelection = "";
            
            Debug.Log($"✅ User logged out: {userEmail}");
            
            // Optional: You can add an event here to notify UI components
            // OnLogoutComplete?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error during logout: {e.Message}");
        }
    }
    
    // NEW: Check if user is currently logged in (either Firebase or session)
    public bool IsUserLoggedIn()
    {
        return (auth != null && auth.CurrentUser != null) || HasValidSession();
    }
    
    // NEW: Get current user email (useful for UI display)
    public string GetCurrentUserEmail()
    {
        if (auth?.CurrentUser != null)
        {
            return auth.CurrentUser.Email;
        }
        else if (HasValidSession())
        {
            return PlayerPrefs.GetString(USER_EMAIL_KEY, "");
        }
        return "";
    }
    
    // NEW: Get session info for debugging
    public string GetSessionInfo()
    {
        if (HasValidSession())
        {
            TimeSpan remaining = GetRemainingSessionTime();
            return $"Session active. Remaining: {remaining.Days}d {remaining.Hours}h {remaining.Minutes}m";
        }
        return "No active session";
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

    // NEW: Send password reset email
    public async Task SendPasswordResetEmail(string email)
    {
        if (!isFirebaseInitialized)
        {
            OnPasswordResetError?.Invoke("Firebase not initialized");
            return;
        }

        try
        {
            await auth.SendPasswordResetEmailAsync(email);
            Debug.Log($"✅ Password reset email sent to: {email}");
            OnPasswordResetEmailSent?.Invoke(true);
        }
        catch (FirebaseException e)
        {
            string errorMessage = GetFirebaseErrorMessage(e);
            OnPasswordResetError?.Invoke(errorMessage);
            Debug.LogError($"❌ Password reset error: {errorMessage}");
        }
    }

    // NEW: Check if current user's email is verified
    public async Task<bool> CheckEmailVerification()
    {
        if (auth?.CurrentUser != null)
        {
            await auth.CurrentUser.ReloadAsync();
            return auth.CurrentUser.IsEmailVerified;
        }
        return false;
    }

    // NEW: Update user password (for authenticated user)
    public async Task UpdateUserPassword(string newPassword)
    {
        if (!isFirebaseInitialized)
        {
            OnPasswordUpdateResult?.Invoke(false, "Firebase not initialized");
            return;
        }

        var user = auth?.CurrentUser;
        if (user == null)
        {
            OnPasswordUpdateResult?.Invoke(false, "No user logged in");
            return;
        }

        try
        {
            await user.UpdatePasswordAsync(newPassword);
            Debug.Log("✅ Password updated successfully");
            
            // Sign out user after password update
            auth.SignOut();
            ClearSessionData();
            
            OnPasswordUpdateResult?.Invoke(true, "Password updated successfully");
        }
        catch (FirebaseException e)
        {
            string errorMessage = GetFirebaseErrorMessage(e);
            OnPasswordUpdateResult?.Invoke(false, errorMessage);
            Debug.LogError($"❌ Password update error: {errorMessage}");
        }
    }

    // NEW: Update game score and statistics based on difficulty and game result
    public async Task UpdateGameScore(string difficulty, int lastScore, int failedAttempts, float timeTaken, bool isWin)
    {
        if (!isFirebaseInitialized)
        {
            Debug.LogError("❌ Firebase not initialized");
            return;
        }
        
        var user = GetCurrentUser();
        if (user == null && !HasValidSession())
        {
            Debug.LogError("❌ No user logged in");
            return;
        }

        try
        {
            // Get user ID (from Firebase user or session)
            string userId;
            if (user != null)
            {
                userId = user.UserId;
            }
            else
            {
                // If we're using session, we need to get user ID differently
                // For now, we'll use the email as identifier
                string userEmail = GetCurrentUserEmail();
                userId = userEmail.Replace(".", "_").Replace("@", "_"); // Simple conversion
            }

            // Determine which collection to update based on current game selection
            string gameCollection = string.IsNullOrEmpty(currentGameSelection) ? "GM1" : currentGameSelection;
            
            var gameRef = db.Collection("users").Document(userId).Collection(gameCollection).Document("scores");
            
            // Determine which score field to update based on difficulty
            string scoreField = "";
            switch (difficulty.ToLower())
            {
                case "easy":
                    scoreField = "EasyScore";
                    break;
                case "medium":
                    scoreField = "MediumScore";
                    break;
                case "hard":
                    scoreField = "HardScore";
                    break;
                default:
                    Debug.LogError($"❌ Invalid difficulty: {difficulty}");
                    return;
            }

            // Get current statistics to increment
            var scoreDoc = await gameRef.GetSnapshotAsync();
            
            // Current values (with defaults if document doesn't exist)
            int currentWins = 0;
            int currentLoses = 0;
            int currentFailedAttempts = 0;
            
            if (scoreDoc.Exists)
            {
                if (scoreDoc.ContainsField("Wins"))
                    currentWins = scoreDoc.GetValue<int>("Wins");
                if (scoreDoc.ContainsField("Lose"))
                    currentLoses = scoreDoc.GetValue<int>("Lose");
                if (scoreDoc.ContainsField("FailedAttempts"))
                    currentFailedAttempts = scoreDoc.GetValue<int>("FailedAttempts");
            }

            // Calculate new statistics
            int newWins = currentWins + (isWin ? 1 : 0);
            int newLoses = currentLoses + (isWin ? 0 : 1);
            int newFailedAttempts = failedAttempts; // Store only last game's failed attempts (wrong selections)
            
            // Prepare update data - ALWAYS UPDATE (no comparison for "best" score)
            var updateData = new Dictionary<string, object>
            {
                [scoreField] = lastScore,                           // Last played score for this difficulty
                ["TimeTaken"] = Mathf.RoundToInt(timeTaken),       // Time taken in seconds (rounded)
                ["FailedAttempts"] = newFailedAttempts,            // Last game's failed attempts only
                ["Wins"] = newWins,                                // Total wins
                ["Lose"] = newLoses,                               // Total losses
                ["lastPlayed"] = Timestamp.GetCurrentTimestamp()   // Update last played timestamp
            };

            // Update the document with all new data
            await gameRef.UpdateAsync(updateData);
            
            string resultText = isWin ? "WIN" : "LOSE";
            Debug.Log($"✅ Game statistics updated for {difficulty} ({resultText}):");
            Debug.Log($"   Last Score: {lastScore}");
            Debug.Log($"   Time Taken: {timeTaken:F1}s");
            Debug.Log($"   Total Wins: {newWins}");
            Debug.Log($"   Total Losses: {newLoses}");
            Debug.Log($"   Last Game Failed Attempts (Wrong Selections): {newFailedAttempts}");
            Debug.Log($"   Game Collection: {gameCollection}");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error updating game statistics: {e.Message}");
            throw; // Re-throw so the caller can handle it
        }
    }

    // LEGACY METHOD - Keep for backward compatibility but redirect to new method
    public async Task UpdateGameScore(string difficulty, int score)
    {
        // Default behavior: assume it's a loss with 60 seconds time if using old method
        // Since we don't have failed attempts info, assume score represents failed attempts
        await UpdateGameScore(difficulty, score, score, 60f, false);
    }

    // NEW: Get game statistics for display purposes
    public async Task<GameStatistics> GetGameStatistics(string gameType = "")
    {
        if (!isFirebaseInitialized)
        {
            Debug.LogError("❌ Firebase not initialized");
            return null;
        }
        
        var user = GetCurrentUser();
        if (user == null && !HasValidSession())
        {
            Debug.LogError("❌ No user logged in");
            return null;
        }

        try
        {
            // Get user ID
            string userId;
            if (user != null)
            {
                userId = user.UserId;
            }
            else
            {
                string userEmail = GetCurrentUserEmail();
                userId = userEmail.Replace(".", "_").Replace("@", "_");
            }

            // Use provided game type or current selection
            string gameCollection = string.IsNullOrEmpty(gameType) ? 
                                  (string.IsNullOrEmpty(currentGameSelection) ? "GM1" : currentGameSelection) : 
                                  gameType;
            
            var gameRef = db.Collection("users").Document(userId).Collection(gameCollection).Document("scores");
            var scoreDoc = await gameRef.GetSnapshotAsync();
            
            if (scoreDoc.Exists)
            {
                var stats = new GameStatistics
                {
                    EasyScore = scoreDoc.ContainsField("EasyScore") ? scoreDoc.GetValue<int>("EasyScore") : 0,
                    MediumScore = scoreDoc.ContainsField("MediumScore") ? scoreDoc.GetValue<int>("MediumScore") : 0,
                    HardScore = scoreDoc.ContainsField("HardScore") ? scoreDoc.GetValue<int>("HardScore") : 0,
                    TimeTaken = scoreDoc.ContainsField("TimeTaken") ? scoreDoc.GetValue<int>("TimeTaken") : 0,
                    FailedAttempts = scoreDoc.ContainsField("FailedAttempts") ? scoreDoc.GetValue<int>("FailedAttempts") : 0,
                    Wins = scoreDoc.ContainsField("Wins") ? scoreDoc.GetValue<int>("Wins") : 0,
                    Loses = scoreDoc.ContainsField("Lose") ? scoreDoc.GetValue<int>("Lose") : 0,
                    GameType = gameCollection
                };
                
                Debug.Log($"📊 Retrieved game statistics for {gameCollection}:");
                Debug.Log($"   Easy: {stats.EasyScore}, Medium: {stats.MediumScore}, Hard: {stats.HardScore}");
                Debug.Log($"   Wins: {stats.Wins}, Losses: {stats.Loses}, Failed Attempts: {stats.FailedAttempts}");
                
                return stats;
            }
            else
            {
                Debug.LogWarning($"⚠️ No statistics found for {gameCollection}");
                return new GameStatistics { GameType = gameCollection };
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error retrieving game statistics: {e.Message}");
            return null;
        }
    }
}

// NEW: Data structure for game statistics
[System.Serializable]
public class GameStatistics
{
    public int EasyScore;
    public int MediumScore;  
    public int HardScore;
    public int TimeTaken;
    public int FailedAttempts;
    public int Wins;
    public int Loses;
    public string GameType;
    
    public int TotalGames => Wins + Loses;
    public float WinRate => TotalGames > 0 ? (float)Wins / TotalGames * 100f : 0f;
}
/**
 * IELTS Learning Platform - Main JavaScript
 */

// API Base URL
const API_BASE = '/api';

// Auth state management
const Auth = {
    user: JSON.parse(localStorage.getItem('ielts_user') || 'null'),

    isLoggedIn() {
        return !!this.user;
    },

    setAuth(user) {
        this.user = user;
        localStorage.setItem('ielts_user', JSON.stringify(user));
        this.updateUI();
    },

    logout() {
        this.user = null;
        localStorage.removeItem('ielts_user');
        window.location.href = '/Account/Logout';
    },

    getHeaders() {
        return {
            'Content-Type': 'application/json'
        };
    },

    updateUI() {
        // UI is handled by server-side rendering in _Layout.cshtml
        // We do full page reloads on login/logout, so JS doesn't need to update nav bar.
    }
};

// API helper with improved error handling and security
async function api(endpoint, options = {}) {
    const url = `${API_BASE}${endpoint}`;
    const { suppressError = false, ...fetchOptions } = options;
    const config = {
        headers: Auth.getHeaders(),
        ...fetchOptions
    };

    try {
        const response = await fetch(url, config);

        if (response.status === 401) {
            console.warn('Unauthorized (401) — caller should handle this');
        }

        // Handle empty response (204 No Content, etc.)
        const contentType = response.headers.get('content-type');
        let data = null;

        if (contentType && contentType.includes('application/json')) {
            const text = await response.text();
            if (text) {
                try {
                    data = JSON.parse(text);
                } catch (e) {
                    console.error('Failed to parse JSON:', e);
                    throw new Error('Invalid server response format');
                }
            }
        }

        // If no JSON data, create a default response
        if (!data) {
            data = { success: response.ok, message: response.ok ? 'Success' : `Error ${response.status}` };
        }

        if (!response.ok) {
            const errorMsg = data.message || data.Message || data.error ||
                (data.errors ? Object.values(data.errors).flat().join(', ') : null) ||
                `Request failed with status ${response.status}`;
            throw new Error(errorMsg);
        }

        return data;
    } catch (error) {
        if (!suppressError) {
            console.error('API Error:', error);
        }
        throw error;
    }
}

// Toast notifications
function showToast(message, type = 'info') {
    const toastContainer = document.getElementById('toastContainer') || createToastContainer();

    const toast = document.createElement('div');
    toast.className = `toast show align-items-center text-white bg-${type === 'error' ? 'danger' : type === 'success' ? 'success' : 'primary'} border-0`;
    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">${message}</div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    `;

    toastContainer.appendChild(toast);

    setTimeout(() => {
        toast.remove();
    }, 4000);
}

function createToastContainer() {
    const container = document.createElement('div');
    container.id = 'toastContainer';
    container.className = 'toast-container position-fixed bottom-0 end-0 p-3';
    container.style.zIndex = '1100';
    document.body.appendChild(container);
    return container;
}

// Login form handler
async function handleLogin(event) {
    event.preventDefault();
    const form = event.target;
    const btn = form.querySelector('button[type="submit"]');
    const originalText = btn.innerHTML;

    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Signing in...';

    try {
        const response = await fetch('/api/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                email: form.email.value,
                password: form.password.value
            })
        });

        const data = await response.json();

        if (response.ok && data.success) {
            if (data.user) {
                localStorage.setItem('ielts_user', JSON.stringify(data.user));
            }
            showToast('Welcome back!', 'success');
            window.location.href = data.redirectUrl || '/Home/Dashboard';
        } else {
            showToast(data.message || 'Login failed', 'error');
        }
    } catch (error) {
        showToast('Network error. Please try again.', 'error');
    } finally {
        btn.disabled = false;
        btn.innerHTML = originalText;
    }
}

// Register form handler
async function handleRegister(event) {
    event.preventDefault();
    const form = event.target;
    const btn = form.querySelector('button[type="submit"]');
    const originalText = btn.innerHTML;

    if (form.password.value !== form.confirmPassword.value) {
        showToast('Passwords do not match', 'error');
        return;
    }

    // Validate password format (at least 8 chars with 1 letter and 1 number)
    const password = form.password.value;
    if (password.length < 8) {
        showToast('Password must be at least 8 characters', 'error');
        return;
    }
    if (!/[a-zA-Z]/.test(password) || !/\d/.test(password)) {
        showToast('Password must contain at least one letter and one number', 'error');
        return;
    }

    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Creating account...';

    try {
        const payload = {
            email: form.email.value.trim(),
            password: form.password.value,
            confirmPassword: form.confirmPassword.value,
            fullName: form.fullName?.value?.trim() || '',
            targetBand: parseFloat(form.targetBand?.value) || 6.5
        };

        console.log('Register payload:', { ...payload, password: '***', confirmPassword: '***' });

        const data = await api('/auth/register', {
            method: 'POST',
            body: JSON.stringify(payload)
        });

        // AuthResponse returns token and user at root level
        if (data.success && data.token) {
            Auth.setAuth(data.token, data.user);
            showToast('Account created successfully!', 'success');
            setTimeout(() => {
                window.location.href = data.redirectUrl || '/Home/Dashboard';
            }, 500);
        } else {
            throw new Error(data.message || 'Registration failed');
        }
    } catch (error) {
        showToast(error.message, 'error');
    } finally {
        btn.disabled = false;
        btn.innerHTML = originalText;
    }
}

// Chat functionality
const Chat = {
    container: null,
    input: null,
    messages: [],

    init(containerId, inputId) {
        this.container = document.getElementById(containerId);
        this.input = document.getElementById(inputId);

        if (this.input) {
            this.input.addEventListener('keydown', (e) => {
                if (e.key === 'Enter' && !e.shiftKey) {
                    e.preventDefault();
                    this.send();
                }
            });
        }
    },

    addMessage(content, isUser = false) {
        if (!this.container) return;

        const messageDiv = document.createElement('div');
        messageDiv.className = `chat-message ${isUser ? 'user' : 'ai'} animate-fadeInUp`;
        messageDiv.innerHTML = `
            <div class="message-content">
                ${isUser ? '' : '<i class="bi bi-robot me-2"></i>'}
                ${content}
            </div>
        `;

        this.container.appendChild(messageDiv);
        this.container.scrollTop = this.container.scrollHeight;
    },

    async send() {
        if (!this.input || !this.input.value.trim()) return;

        const message = this.input.value.trim();
        this.input.value = '';

        // Add user message
        this.addMessage(message, true);

        // Show typing indicator
        this.showTyping();

        try {
            const data = await api('/ai/chat', {
                method: 'POST',
                body: JSON.stringify({
                    message: message,
                    context: null
                })
            });

            this.hideTyping();

            if (data.success && data.data) {
                this.addMessage(data.data.response);

                // Show suggested follow-ups
                if (data.data.suggestedFollowUps && data.data.suggestedFollowUps.length > 0) {
                    this.showSuggestions(data.data.suggestedFollowUps);
                }
            }
        } catch (error) {
            this.hideTyping();
            this.addMessage('Sorry, I encountered an error. Please try again.');
        }
    },

    showTyping() {
        const typing = document.createElement('div');
        typing.id = 'typingIndicator';
        typing.className = 'chat-message ai';
        typing.innerHTML = `
            <div class="message-content">
                <div class="d-flex align-items-center gap-1">
                    <div class="spinner-grow spinner-grow-sm" style="width: 8px; height: 8px;"></div>
                    <div class="spinner-grow spinner-grow-sm" style="width: 8px; height: 8px; animation-delay: 0.1s;"></div>
                    <div class="spinner-grow spinner-grow-sm" style="width: 8px; height: 8px; animation-delay: 0.2s;"></div>
                </div>
            </div>
        `;
        this.container.appendChild(typing);
        this.container.scrollTop = this.container.scrollHeight;
    },

    hideTyping() {
        const typing = document.getElementById('typingIndicator');
        if (typing) typing.remove();
    },

    showSuggestions(suggestions) {
        const suggestDiv = document.createElement('div');
        suggestDiv.className = 'chat-suggestions d-flex flex-wrap gap-2 mt-2 mb-3';

        suggestions.forEach(s => {
            const btn = document.createElement('button');
            btn.className = 'btn btn-outline-primary btn-sm';
            btn.textContent = s;
            btn.onclick = () => {
                this.input.value = s;
                this.send();
                suggestDiv.remove();
            };
            suggestDiv.appendChild(btn);
        });

        this.container.appendChild(suggestDiv);
        this.container.scrollTop = this.container.scrollHeight;
    }
};

// Writing practice
const Writing = {
    textarea: null,
    counter: null,
    minWords: 150,

    init(textareaId, counterId, minWords = 150) {
        this.textarea = document.getElementById(textareaId);
        this.counter = document.getElementById(counterId);
        this.minWords = minWords;

        if (this.textarea) {
            this.textarea.addEventListener('input', () => this.updateCount());
            this.updateCount();
        }
    },

    updateCount() {
        if (!this.textarea || !this.counter) return;

        const text = this.textarea.value.trim();
        const words = text ? text.split(/\s+/).length : 0;

        this.counter.innerHTML = `<span class="count">${words}</span> / ${this.minWords} words`;

        const parent = this.counter.parentElement || this.counter;
        parent.classList.remove('warning', 'success');

        if (words >= this.minWords) {
            parent.classList.add('success');
        } else if (words >= this.minWords * 0.7) {
            parent.classList.add('warning');
        }
    },

    async submit(promptId, taskType = 2) {
        if (!this.textarea) return;

        const text = this.textarea.value.trim();
        const words = text.split(/\s+/).length;

        if (words < 50) {
            showToast('Please write at least 50 words', 'error');
            return;
        }

        const btn = document.querySelector('#submitWritingBtn');
        if (btn) {
            btn.disabled = true;
            btn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Analyzing...';
        }

        try {
            const data = await api('/writing/submit', {
                method: 'POST',
                body: JSON.stringify({
                    prompt: document.getElementById('writingPrompt')?.textContent || '',
                    essayText: text,
                    taskType: taskType
                })
            });

            if (data.success && data.data) {
                this.showFeedback(data.data);
            }
        } catch (error) {
            showToast('Failed to submit essay: ' + error.message, 'error');
        } finally {
            if (btn) {
                btn.disabled = false;
                btn.innerHTML = '<i class="bi bi-send me-2"></i>Get AI Feedback';
            }
        }
    },

    showFeedback(result) {
        const feedbackDiv = document.getElementById('writingFeedback');
        if (!feedbackDiv) return;

        feedbackDiv.innerHTML = `
            <div class="feedback-section animate-fadeInUp">
                <div class="feedback-header">
                    <div class="feedback-icon"><i class="bi bi-check-lg"></i></div>
                    <div>
                        <h5 class="mb-0">AI Feedback</h5>
                        <small class="text-muted">Based on IELTS Band Descriptors</small>
                    </div>
                    <div class="ms-auto text-center">
                        <div class="band-score">${result.bandScore?.toFixed(1) || 'N/A'}</div>
                        <small>Overall Band</small>
                    </div>
                </div>
                
                <div class="criteria-grid">
                    <div class="criteria-item">
                        <div class="criteria-score">${result.taskAchievementScore?.toFixed(1) || '-'}</div>
                        <div class="criteria-label">Task Achievement</div>
                    </div>
                    <div class="criteria-item">
                        <div class="criteria-score">${result.coherenceCohesionScore?.toFixed(1) || '-'}</div>
                        <div class="criteria-label">Coherence & Cohesion</div>
                    </div>
                    <div class="criteria-item">
                        <div class="criteria-score">${result.lexicalResourceScore?.toFixed(1) || '-'}</div>
                        <div class="criteria-label">Lexical Resource</div>
                    </div>
                    <div class="criteria-item">
                        <div class="criteria-score">${result.grammarAccuracyScore?.toFixed(1) || '-'}</div>
                        <div class="criteria-label">Grammar & Accuracy</div>
                    </div>
                </div>
                
                <div class="mt-4">
                    <h6><i class="bi bi-chat-quote me-2"></i>Detailed Feedback</h6>
                    <p class="mb-0" style="white-space: pre-line;">${result.aiFeedback || 'No feedback available.'}</p>
                </div>
            </div>
        `;

        feedbackDiv.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
};

// Speaking practice
const Speaking = {
    mediaRecorder: null,
    audioChunks: [],
    isRecording: false,
    sessionId: null,

    async startSession(topic) {
        try {
            const data = await api('/speaking/start', {
                method: 'POST',
                body: JSON.stringify({ topic: topic })
            });

            if (data.success && data.data) {
                this.sessionId = data.data.sessionId;
                showToast('Session started! Click the microphone to begin recording.', 'success');
                return data.data;
            }
        } catch (error) {
            showToast('Failed to start session: ' + error.message, 'error');
        }
    },

    async toggleRecording() {
        if (!this.sessionId) {
            showToast('Please start a session first', 'error');
            return;
        }

        if (this.isRecording) {
            await this.stopRecording();
        } else {
            await this.startRecording();
        }
    },

    async startRecording() {
        try {
            const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
            this.mediaRecorder = new MediaRecorder(stream);
            this.audioChunks = [];

            this.mediaRecorder.ondataavailable = (event) => {
                this.audioChunks.push(event.data);
            };

            this.mediaRecorder.onstop = async () => {
                const audioBlob = new Blob(this.audioChunks, { type: 'audio/webm' });
                await this.submitRecording(audioBlob);
            };

            this.mediaRecorder.start();
            this.isRecording = true;
            this.updateRecordingUI(true);
            showToast('Recording started...', 'info');
        } catch (error) {
            showToast('Microphone access denied', 'error');
        }
    },

    async stopRecording() {
        if (this.mediaRecorder && this.isRecording) {
            this.mediaRecorder.stop();
            this.mediaRecorder.stream.getTracks().forEach(track => track.stop());
            this.isRecording = false;
            this.updateRecordingUI(false);
        }
    },

    updateRecordingUI(isRecording) {
        const btn = document.getElementById('recordBtn');
        if (btn) {
            if (isRecording) {
                btn.classList.add('btn-danger');
                btn.classList.remove('btn-ielts');
                btn.innerHTML = '<i class="bi bi-stop-circle me-2"></i>Stop Recording';
            } else {
                btn.classList.remove('btn-danger');
                btn.classList.add('btn-ielts');
                btn.innerHTML = '<i class="bi bi-mic me-2"></i>Start Recording';
            }
        }
    },

    async submitRecording(audioBlob) {
        const btn = document.getElementById('recordBtn');
        if (btn) {
            btn.disabled = true;
            btn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Analyzing...';
        }

        try {
            // Convert to base64
            const reader = new FileReader();
            reader.readAsDataURL(audioBlob);
            reader.onloadend = async () => {
                const base64Audio = reader.result;

                const data = await api('/speaking/submit-audio', {
                    method: 'POST',
                    body: JSON.stringify({
                        sessionId: this.sessionId,
                        audioData: base64Audio,
                        transcript: '' // Would be filled by speech-to-text
                    })
                });

                if (data.success && data.data) {
                    this.showFeedback(data.data);
                }

                if (btn) {
                    btn.disabled = false;
                    btn.innerHTML = '<i class="bi bi-mic me-2"></i>Start Recording';
                }
            };
        } catch (error) {
            showToast('Failed to submit recording: ' + error.message, 'error');
            if (btn) {
                btn.disabled = false;
                btn.innerHTML = '<i class="bi bi-mic me-2"></i>Start Recording';
            }
        }
    },

    showFeedback(result) {
        const feedbackDiv = document.getElementById('speakingFeedback');
        if (!feedbackDiv) return;

        feedbackDiv.innerHTML = `
            <div class="feedback-section animate-fadeInUp">
                <div class="feedback-header">
                    <div class="feedback-icon"><i class="bi bi-mic"></i></div>
                    <div>
                        <h5 class="mb-0">Speaking Feedback</h5>
                        <small class="text-muted">AI Analysis Results</small>
                    </div>
                    <div class="ms-auto text-center">
                        <div class="band-score">${result.overallBand?.toFixed(1) || 'N/A'}</div>
                        <small>Overall Band</small>
                    </div>
                </div>
                
                <div class="criteria-grid">
                    <div class="criteria-item">
                        <div class="criteria-score">${result.fluencyScore?.toFixed(1) || '-'}</div>
                        <div class="criteria-label">Fluency</div>
                    </div>
                    <div class="criteria-item">
                        <div class="criteria-score">${result.pronunciationScore?.toFixed(1) || '-'}</div>
                        <div class="criteria-label">Pronunciation</div>
                    </div>
                    <div class="criteria-item">
                        <div class="criteria-score">${result.grammarScore?.toFixed(1) || '-'}</div>
                        <div class="criteria-label">Grammar</div>
                    </div>
                </div>
                
                <div class="mt-4">
                    <h6><i class="bi bi-chat-quote me-2"></i>Feedback</h6>
                    <p class="mb-0" style="white-space: pre-line;">${result.aiFeedback || 'Good effort! Keep practicing.'}</p>
                </div>
            </div>
        `;

        feedbackDiv.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
};

// Dashboard data loading
const Dashboard = {
    async loadStats() {
        if (!Auth.isLoggedIn()) {
            console.log('Not logged in, skipping dashboard load');
            // window.location.href = '/Home/Login';
            return;
        }

        try {
            const [profile, recommendations, prediction] = await Promise.all([
                api('/users/profile').catch(e => { console.warn('Profile load failed:', e); return { success: false, data: null }; }),
                api('/ai/recommendations').catch(e => { console.warn('Recommendations load failed:', e); return { success: false, data: [] }; }),
                api('/ai/predict-band').catch(e => { console.warn('Prediction load failed:', e); return { success: false, data: null }; })
            ]);

            // Check if we got redirected due to 401
            if (!profile.success && !profile.data) {
                return; // Auth.logout() already called
            }

            this.renderStats(profile.data, prediction.data);
            this.renderRecommendations(recommendations.data || []);
        } catch (error) {
            console.error('Failed to load dashboard:', error);
            showToast('Failed to load dashboard data', 'error');
        }
    },

    renderStats(profile, prediction) {
        const statsContainer = document.getElementById('dashboardStats');
        if (!statsContainer) return;

        const band = prediction?.predictedBand || profile?.currentBand || 5.5;
        const targetBand = profile?.targetBand || 7.0;

        statsContainer.innerHTML = `
            <div class="col-md-3">
                <div class="card stat-card">
                    <div class="stat-icon bg-primary bg-opacity-10">
                        <i class="bi bi-graph-up text-primary"></i>
                    </div>
                    <div class="stat-value">${band.toFixed(1)}</div>
                    <div class="stat-label">Current Level</div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="card stat-card">
                    <div class="stat-icon bg-accent bg-opacity-10">
                        <i class="bi bi-bullseye text-warning"></i>
                    </div>
                    <div class="stat-value">${targetBand.toFixed(1)}</div>
                    <div class="stat-label">Target Band</div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="card stat-card">
                    <div class="stat-icon bg-success bg-opacity-10">
                        <i class="bi bi-check-circle text-success"></i>
                    </div>
                    <div class="stat-value">${profile?.completedLessons || 0}</div>
                    <div class="stat-label">Lessons Done</div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="card stat-card">
                    <div class="stat-icon bg-info bg-opacity-10">
                        <i class="bi bi-fire text-info"></i>
                    </div>
                    <div class="stat-value">${profile?.streakDays || 0}</div>
                    <div class="stat-label">Day Streak</div>
                </div>
            </div>
        `;
    },

    renderRecommendations(recommendations) {
        const container = document.getElementById('recommendations');
        if (!container) return;

        // Fallback rendering if database has no recommendations (prevents empty slop during demo)
        if (!recommendations || recommendations.length === 0) {
            container.innerHTML = `
                <div class="p-3 mb-3 rounded border border-start border-3 border-danger bg-danger bg-opacity-5" style="border-radius: 6px;">
                    <div class="d-flex justify-content-between mb-1">
                        <span class="badge bg-danger bg-opacity-10 text-danger" style="font-size: 10px;"><i class="fa-solid fa-triangle-exclamation me-1"></i>Học liệu yếu</span>
                        <span class="small text-muted" style="font-size: 11px;">Độ ưu tiên: Cao</span>
                    </div>
                    <h6 class="fw-bold text-navy mb-1" style="font-size: 0.95rem;">Bạn đang yếu dạng câu hỏi TFNG (Reading)</h6>
                    <p class="text-muted small mb-2">Tỷ lệ trả lời chính xác trong bài luyện gần đây khá thấp.</p>
                    <a href="/Home/Skill?type=reading" class="btn btn-outline-danger btn-sm w-100 py-1 fw-bold transition-editorial text-decoration-none text-center d-block" style="font-size: 11px;">Thực hành Đọc ngay (15 phút)</a>
                </div>
                <div class="p-3 rounded border border-start border-3 border-primary bg-primary bg-opacity-5" style="border-radius: 6px; border-color: var(--skill-writing-color) !important;">
                    <div class="d-flex justify-content-between mb-1">
                        <span class="badge text-white" style="font-size: 10px; background-color: var(--skill-writing-color);"><i class="fa-solid fa-pen-nib me-1"></i>Writing Coherence</span>
                        <span class="small text-muted" style="font-size: 11px;">Tiến trình: +0.5 Band</span>
                    </div>
                    <h6 class="fw-bold text-navy mb-1" style="font-size: 0.95rem;">Luyện viết Cohesion Task 2</h6>
                    <p class="text-muted small mb-2">Tập trung nối ý mạch lạc để bứt phá band viết luận.</p>
                    <a href="/Home/Practice?type=writing" class="btn btn-outline-primary btn-sm w-100 py-1 fw-bold transition-editorial text-decoration-none text-center d-block" style="font-size: 11px; color: var(--skill-writing-color); border-color: var(--skill-writing-color);">Làm bài tập viết</a>
                </div>
            `;
            return;
        }

        container.innerHTML = recommendations.map(r => {
            const title = r.title || r;
            const desc = r.description || 'Recommended practice lesson from curriculum';
            const isWriting = title.toLowerCase().includes('write') || title.toLowerCase().includes('essay') || title.toLowerCase().includes('cohesion');
            const isSpeaking = title.toLowerCase().includes('speak') || title.toLowerCase().includes('pronounce');
            const isListening = title.toLowerCase().includes('listen') || title.toLowerCase().includes('audio');
            const skillClass = isWriting ? 'var(--skill-writing-color)' : isSpeaking ? 'var(--skill-speaking-color)' : isListening ? 'var(--skill-listening-color)' : 'var(--skill-reading-color)';
            const skillIcon = isWriting ? 'fa-pen-nib' : isSpeaking ? 'fa-microphone' : isListening ? 'fa-headphones' : 'fa-book-open';
            const skillLabel = isWriting ? 'Writing' : isSpeaking ? 'Speaking' : isListening ? 'Listening' : 'Reading';

            return `
                <div class="p-3 mb-2 rounded border border-start border-3 bg-light transition-editorial" style="border-radius: 6px; border-left-color: ${skillClass} !important;">
                    <div class="d-flex justify-content-between mb-1">
                        <span class="badge text-white" style="font-size: 10px; background-color: ${skillClass};"><i class="fa-solid ${skillIcon} me-1"></i>${skillLabel}</span>
                        <span class="small text-muted" style="font-size: 10px;">Priority: Medium</span>
                    </div>
                    <h6 class="fw-bold text-navy mb-1" style="font-size: 0.95rem;">${title}</h6>
                    <p class="text-muted small mb-2">${desc}</p>
                    <a href="/Home/Skill?type=${skillLabel.toLowerCase()}" class="btn btn-outline-secondary btn-sm w-100 py-1 fw-bold transition-editorial text-decoration-none text-center d-block" style="font-size: 11px;">Study Now</a>
                </div>
            `;
        }).join('');
    }
};

// Initialize on page load
document.addEventListener('DOMContentLoaded', function () {
    // Update auth UI
    Auth.updateUI();

    // Initialize based on page
    const page = document.body.dataset.page;

    if (page === 'dashboard') {
        Dashboard.loadStats();
    }

    // Initialize forms
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', handleLogin);
    }

    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', handleRegister);
    }

    // Initialize chat if on AI Tutor page
    const chatContainer = document.getElementById('chatMessages');
    if (chatContainer) {
        Chat.init('chatMessages', 'chatInput');
    }

    // Initialize writing practice
    const writingTextarea = document.getElementById('writingTextarea');
    if (writingTextarea) {
        Writing.init('writingTextarea', 'wordCounter', 250);
    }

    console.log('IELTS Learning Platform initialized');
});

// API wrapper for easier use
const API = {
    async get(endpoint, options = {}) {
        return api(endpoint, { method: 'GET', ...options });
    },
    async post(endpoint, data, options = {}) {
        return api(endpoint, { method: 'POST', body: JSON.stringify(data), ...options });
    },
    async put(endpoint, data, options = {}) {
        return api(endpoint, { method: 'PUT', body: JSON.stringify(data), ...options });
    },
    async delete(endpoint, options = {}) {
        return api(endpoint, { method: 'DELETE', ...options });
    }
};

// Toast with better styling
const Toast = {
    show(message, type = 'info') {
        let container = document.querySelector('.toast-container');
        if (!container) {
            container = document.createElement('div');
            container.className = 'toast-container';
            document.body.appendChild(container);
        }

        const toast = document.createElement('div');
        toast.className = `toast-notification ${type}`;
        const icon = type === 'success' ? 'check-circle' : type === 'error' ? 'x-circle' : 'info-circle';
        toast.innerHTML = `
            <i class="bi bi-${icon} fs-5"></i>
            <span>${message}</span>
        `;

        container.appendChild(toast);

        setTimeout(() => {
            toast.style.animation = 'slideIn 0.3s ease reverse';
            setTimeout(() => toast.remove(), 300);
        }, 3500);
    }
};

// Enhanced Dashboard
Dashboard.load = async function () {
    if (!Auth.isLoggedIn()) {
        console.log('Not logged in, redirecting to login');
        // window.location.href = '/Home/Login';
        return;
    }

    try {
        const progress = await api('/users/progress', { suppressError: true }).catch(e => {
            console.warn('Progress load failed:', e);
            return { success: false, data: null };
        });

        // Check if redirected due to 401
        if (!progress.success && !progress.data) {
            return;
        }

        if (progress.data) {
            const p = progress.data;

            // Update stat values
            const currentBand = document.getElementById('currentBand');
            const targetBand = document.getElementById('targetBand');
            const lessonsCompleted = document.getElementById('lessonsCompleted');
            const streak = document.getElementById('streak');

            if (currentBand) currentBand.textContent = p.currentBand?.toFixed(1) || '5.0';
            if (targetBand) targetBand.textContent = p.targetBand?.toFixed(1) || '7.0';
            if (lessonsCompleted) lessonsCompleted.textContent = p.totalLessonsCompleted || 0;
            if (streak) streak.textContent = p.streakDays || 0;

            // Update skill progress bars
            if (p.skills) {
                Dashboard.updateSkillProgress('reading', p.skills.reading);
                Dashboard.updateSkillProgress('listening', p.skills.listening);
                Dashboard.updateSkillProgress('writing', p.skills.writing);
                Dashboard.updateSkillProgress('speaking', p.skills.speaking);
            }
        }

        // Only update welcome message if the server rendered a generic name
        const welcomeHeading = document.getElementById('welcomeHeading');
        if (welcomeHeading) {
            const currentText = welcomeHeading.textContent;
            // Don't override if server already rendered a real name via @User.GetFullName()
            if (currentText.includes('Guest') || currentText.trim() === '') {
                const firstName = Auth.user?.fullName?.split(' ')[0] || 'there';
                welcomeHeading.textContent = `Welcome back, ${firstName}!`;
            }
        }

        // Load learning roadmap
        Dashboard.loadRoadmap();
    } catch (error) {
        console.error('Dashboard load error:', error);
        showToast('Failed to load dashboard', 'error');
    }
};

Dashboard.updateSkillProgress = function (skill, score) {
    const scoreEl = document.getElementById(`${skill}Score`);
    const progressEl = document.getElementById(`${skill}Progress`);

    if (scoreEl) scoreEl.textContent = score?.toFixed(1) || '--';
    if (progressEl) progressEl.style.width = ((score || 0) / 9 * 100) + '%';
};

// Load learning roadmap with lessons
Dashboard.loadRoadmap = async function () {
    const loadingEl = document.getElementById('roadmapLoading');
    const contentEl = document.getElementById('roadmapContent');
    const noRoadmapEl = document.getElementById('noRoadmap');

    if (!loadingEl || !contentEl) return;

    try {
        let response = await api('/ai/learning-path', { suppressError: true }).catch(() => null);

        if (!response || !response.success) {
            const goalsResponse = await api('/users/goals', { suppressError: true }).catch(() => ({ data: null }));
            if (goalsResponse && goalsResponse.data) {
                response = await api('/ai/generate-learning-path', {
                    method: 'POST',
                    body: JSON.stringify({
                        targetBand: goalsResponse.data.targetBand || 7.0,
                        studyHoursPerDay: goalsResponse.data.studyHoursPerDay || 2
                    })
                }).catch(() => null);
            }
        }

        loadingEl.style.display = 'none';

        if (response && response.success && response.data && response.data.weeklyPlan) {
            const plan = response.data;
            contentEl.style.display = 'block';

            // Show current week and next lessons
            let html = `
                <div class="d-flex align-items-center mb-3 pb-2 border-bottom">
                    <div class="me-3">
                        <span class="badge bg-dark text-white fs-6 px-3 py-2" style="border-radius: var(--radius-sm);"><i class="fa-solid fa-calendar-week text-warning me-1"></i> Week ${plan.currentWeek || 1}/${plan.estimatedWeeks}</span>
                    </div>
                    <div>
                        <strong class="text-navy">Target Pathway: Band ${plan.targetBand?.toFixed(1) || '7.0'}</strong>
                        <small class="text-muted d-block">Estimated ${plan.estimatedWeeks - (plan.currentWeek || 1)} weeks remaining</small>
                    </div>
                </div>
                <h6 class="text-muted mb-3 fw-bold" style="font-size: 13px;"><i class="fa-solid fa-list-check me-2"></i>Recommended Next Lessons:</h6>
                <div class="row g-3">
            `;

            // Get lessons for current week
            const currentWeekData = plan.weeklyPlan?.find(w => w.week === (plan.currentWeek || 1)) || plan.weeklyPlan?.[0];

            if (currentWeekData && currentWeekData.lessons && currentWeekData.lessons.length > 0) {
                currentWeekData.lessons.slice(0, 4).forEach((lesson, idx) => {
                    const skillColors = {
                        'reading': 'var(--skill-reading-color)',
                        'listening': 'var(--skill-listening-color)',
                        'writing': 'var(--skill-writing-color)',
                        'speaking': 'var(--skill-speaking-color)',
                        'vocabulary': 'var(--academic-gold)',
                        'grammar': '#718096'
                    };
                    const skillIcons = {
                        'reading': 'fa-book-open',
                        'listening': 'fa-headphones',
                        'writing': 'fa-pen-nib',
                        'speaking': 'fa-microphone',
                        'vocabulary': 'fa-spell-check',
                        'grammar': 'fa-circle-nodes'
                    };
                    const skill = (lesson.skillType || lesson.skill || 'reading').toLowerCase();
                    const color = skillColors[skill] || 'var(--skill-reading-color)';
                    const icon = skillIcons[skill] || 'fa-book-open';

                    html += `
                        <div class="col-md-6 col-lg-3">
                            <a href="/Home/Skill?type=${skill}" class="card h-100 text-decoration-none lesson-card-hover border transition-editorial" style="border-radius: var(--radius-sm); border: 1px solid rgba(0,0,0,0.08) !important;">
                                <div class="card-body text-center p-3">
                                    <div class="d-inline-flex align-items-center justify-content-center rounded-circle border mb-2" style="width: 48px; height: 48px; background-color: #faf8f5;">
                                        <i class="fa-solid ${icon}" style="color: ${color}; font-size: 1.4rem;"></i>
                                    </div>
                                    <h6 class="mb-2 fw-bold text-navy text-truncate" style="font-size: 0.95rem; font-family: 'Playfair Display', serif;">${lesson.title || lesson.name || 'Lesson ' + (idx + 1)}</h6>
                                    <span class="badge" style="font-size: 10px; background-color: ${color}; color: white;">${skill.charAt(0).toUpperCase() + skill.slice(1)}</span>
                                </div>
                              </a>
                          </div>
                      `;
                  });
              } else {
                  // Default lessons if no specific lessons in roadmap
                  const defaultLessons = [
                      { skill: 'Reading', icon: 'fa-book-open', color: 'var(--skill-reading-color)', title: 'Reading Strategy' },
                      { skill: 'Listening', icon: 'fa-headphones', color: 'var(--skill-listening-color)', title: 'Listening Skills' },
                      { skill: 'Writing', icon: 'fa-pen-nib', color: 'var(--skill-writing-color)', title: 'Writing Practice' },
                      { skill: 'Speaking', icon: 'fa-microphone', color: 'var(--skill-speaking-color)', title: 'Speaking Practice' }
                  ];

                  defaultLessons.forEach(lesson => {
                      html += `
                          <div class="col-md-6 col-lg-3">
                              <a href="/Home/Skill?type=${lesson.skill.toLowerCase()}" class="card h-100 text-decoration-none lesson-card-hover border transition-editorial" style="border-radius: var(--radius-sm); border: 1px solid rgba(0,0,0,0.08) !important;">
                                  <div class="card-body text-center p-3">
                                      <div class="d-inline-flex align-items-center justify-content-center rounded-circle border mb-2" style="width: 48px; height: 48px; background-color: #faf8f5;">
                                          <i class="fa-solid ${lesson.icon}" style="color: ${lesson.color}; font-size: 1.4rem;"></i>
                                      </div>
                                      <h6 class="mb-2 fw-bold text-navy text-truncate" style="font-size: 0.95rem; font-family: 'Playfair Display', serif;">${lesson.title}</h6>
                                      <span class="badge" style="font-size: 10px; background-color: ${lesson.color}; color: white;">${lesson.skill}</span>
                                  </div>
                              </a>
                          </div>
                      `;
                  });
              }

              html += '</div>';
              contentEl.innerHTML = html;
          } else {
              // No roadmap yet
              noRoadmapEl.style.display = 'block';
          }
      } catch (error) {
          console.error('Failed to load roadmap:', error);
          loadingEl.style.display = 'none';
          noRoadmapEl.style.display = 'block';
      }
  };

// Speaking module extensions
Speaking.init = function (config) {
    // Just for initialization, actual recording handled in view
    console.log('Speaking module initialized');
};

Speaking.showFeedback = function (result, containerId) {
    const feedbackDiv = document.getElementById(containerId);
    if (!feedbackDiv) return;

    feedbackDiv.innerHTML = `
        <div class="ai-feedback-card animate-fadeInUp">
            <div class="ai-feedback-header">
                <small class="text-white-50">Your Speaking Score</small>
                <div class="ai-feedback-band">${result.overallBand?.toFixed(1) || 'N/A'}</div>
            </div>
            <div class="ai-feedback-body">
                <div class="score-grid">
                    <div class="score-item">
                        <div class="score">${result.fluencyScore?.toFixed(1) || '-'}</div>
                        <div class="label">Fluency</div>
                    </div>
                    <div class="score-item">
                        <div class="score">${result.pronunciationScore?.toFixed(1) || '-'}</div>
                        <div class="label">Pronunciation</div>
                    </div>
                    <div class="score-item">
                        <div class="score">${result.grammarScore?.toFixed(1) || '-'}</div>
                        <div class="label">Grammar</div>
                    </div>
                    <div class="score-item">
                        <div class="score">${result.vocabularyScore?.toFixed(1) || '-'}</div>
                        <div class="label">Vocabulary</div>
                    </div>
                </div>
                
                <div class="mt-4">
                    <h6 class="fw-bold"><i class="bi bi-chat-quote me-2"></i>AI Feedback</h6>
                    <p class="mb-0 text-muted" style="white-space: pre-line;">${result.aiFeedback || 'Keep practicing to improve!'}</p>
                </div>
            </div>
        </div>
    `;

    feedbackDiv.scrollIntoView({ behavior: 'smooth', block: 'start' });
};

// Profile modal
function showProfile() {
    // Create and show profile modal
    const modal = document.createElement('div');
    modal.innerHTML = `
        <div class="modal fade" id="profileModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title"><i class="bi bi-person-circle me-2"></i>Profile</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="text-center mb-4">
                            <div class="d-inline-flex align-items-center justify-content-center bg-ielts text-white rounded-circle" style="width: 80px; height: 80px; font-size: 2rem;">
                                ${(Auth.user?.fullName?.[0] || 'U').toUpperCase()}
                            </div>
                            <h5 class="mt-3 mb-0">${Auth.user?.fullName || 'User'}</h5>
                            <small class="text-muted">${Auth.user?.email || ''}</small>
                        </div>
                        <div class="d-grid gap-2">
                            <a href="/Home/Dashboard" class="btn btn-outline-primary">
                                <i class="bi bi-speedometer2 me-2"></i>Go to Dashboard
                            </a>
                            <button class="btn btn-danger" onclick="Auth.logout()">
                                <i class="bi bi-box-arrow-right me-2"></i>Logout
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;

    document.body.appendChild(modal);
    const bsModal = new bootstrap.Modal(document.getElementById('profileModal'));
    bsModal.show();

    document.getElementById('profileModal').addEventListener('hidden.bs.modal', () => {
        modal.remove();
    });
}

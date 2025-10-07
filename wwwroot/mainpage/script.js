class NotificationSystem {
    constructor() {
        this.baseUrl = '/api/notifier';
        this.initEventListeners();
        this.loadInitialData();
    }

    initEventListeners() {
        // CSV file upload
        const csvFileInput = document.getElementById('csvFile');
        const uploadCsvBtn = document.getElementById('uploadCsvBtn');
        const fileUploadLabel = document.getElementById('fileUploadLabel');

        csvFileInput.addEventListener('change', (e) => {
            const file = e.target.files[0];
            if (file) {
                fileUploadLabel.innerHTML = `
                    <i class="fas fa-file-csv" style="font-size: 2rem; margin-bottom: 10px; color: var(--success);"></i>
                    <div style="font-weight: 600;">${file.name}</div>
                    <div style="opacity: 0.7; font-size: 0.9rem;">${this.formatFileSize(file.size)}</div>
                `;
                uploadCsvBtn.disabled = false;
            }
        });

        // Drag and drop
        fileUploadLabel.addEventListener('dragover', (e) => {
            e.preventDefault();
            fileUploadLabel.classList.add('dragover');
        });

        fileUploadLabel.addEventListener('dragleave', () => {
            fileUploadLabel.classList.remove('dragover');
        });

        fileUploadLabel.addEventListener('drop', (e) => {
            e.preventDefault();
            fileUploadLabel.classList.remove('dragover');
            const file = e.dataTransfer.files[0];
            if (file && file.name.endsWith('.csv')) {
                csvFileInput.files = e.dataTransfer.files;
                csvFileInput.dispatchEvent(new Event('change'));
            } else {
                this.showToast('Please select a CSV file', 'error');
            }
        });

        // Buttons
        uploadCsvBtn.addEventListener('click', () => this.uploadCsv());
        document.getElementById('loadUsersBtn').addEventListener('click', () => this.loadUsers());
        document.getElementById('loadNoticesBtn').addEventListener('click', () => this.loadNotices());
        document.getElementById('createNoticeForm').addEventListener('submit', (e) => this.createNotice(e));
    }

    async loadInitialData() {
        await Promise.all([
            this.loadUsers(),
            this.loadNotices()
        ]);
        this.updateStats();
    }

    async uploadCsv() {
        const fileInput = document.getElementById('csvFile');
        const file = fileInput.files[0];
        const uploadBtn = document.getElementById('uploadCsvBtn');

        if (!file) {
            this.showToast('Please select a CSV file first', 'warning');
            return;
        }

        const formData = new FormData();
        formData.append('csvFile', file);

        try {
            this.setButtonLoading(uploadBtn, true);

            const response = await fetch(`${this.baseUrl}/users/upload-csv`, {
                method: 'POST',
                body: formData
            });

            if (response.ok) {
                const result = await response.text();
                this.showToast(result, 'success');
                await this.loadUsers();
                fileInput.value = '';
                document.getElementById('fileUploadLabel').innerHTML = `
                    <i class="fas fa-cloud-upload-alt" style="font-size: 3rem; margin-bottom: 15px; opacity: 0.7;"></i>
                    <div style="font-size: 1.2rem; font-weight: 600;">Choose CSV File or Drag & Drop</div>
                    <div style="opacity: 0.7; margin-top: 5px;">Supports .csv files only</div>
                `;
                uploadBtn.disabled = true;
            } else {
                const error = await response.text();
                throw new Error(error);
            }
        } catch (error) {
            console.error('Upload error:', error);
            this.showToast('Error uploading CSV file', 'error');
        } finally {
            this.setButtonLoading(uploadBtn, false);
        }
    }

    async loadUsers() {
        try {
            const response = await fetch(`${this.baseUrl}/users`);
            if (response.ok) {
                const users = await response.json();
                this.displayUsers(users);
                this.updateStats();
            } else {
                throw new Error('Failed to load users');
            }
        } catch (error) {
            console.error('Error loading users:', error);
            this.showToast('Error loading users', 'error');
        }
    }

    async loadNotices() {
        try {
            const response = await fetch(`${this.baseUrl}/notices`);
            if (response.ok) {
                const notices = await response.json();
                this.displayNotices(notices);
                this.updateStats();
            } else {
                throw new Error('Failed to load notices');
            }
        } catch (error) {
            console.error('Error loading notices:', error);
            this.showToast('Error loading notices', 'error');
        }
    }

    async createNotice(e) {
        e.preventDefault();

        const notifierName = document.getElementById('notifierName').value;
        const message = document.getElementById('message').value;
        const createBtn = document.getElementById('createNoticeBtn');

        if (!notifierName || !message) {
            this.showToast('Please fill all fields', 'warning');
            return;
        }

        const noticeData = {
            notifierName: notifierName,
            message: message
        };

        try {
            this.setButtonLoading(createBtn, true);

            const response = await fetch(`${this.baseUrl}/notices`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(noticeData)
            });

            if (response.ok) {
                this.showToast('Notice created successfully!', 'success');
                document.getElementById('createNoticeForm').reset();
                await this.loadNotices();
            } else {
                const error = await response.text();
                throw new Error(error);
            }
        } catch (error) {
            console.error('Error creating notice:', error);
            this.showToast('Error creating notice', 'error');
        } finally {
            this.setButtonLoading(createBtn, false);
        }
    }

    async notifyUsers(noticeId, notifierName) {
        if (!confirm(`Send notification "${notifierName}" to all users?`)) {
            return;
        }

        try {
            const response = await fetch(`${this.baseUrl}/notify/${noticeId}`, {
                method: 'POST'
            });

            if (response.ok) {
                this.showToast('Users notified successfully!', 'success');
                await this.loadUsers();
                await this.loadNotices();
            } else {
                const error = await response.text();
                throw new Error(error);
            }
        } catch (error) {
            console.error('Error notifying users:', error);
            this.showToast('Error notifying users', 'error');

            // Retry logic
            if (confirm('Failed to notify users. Would you like to retry?')) {
                await this.notifyUsers(noticeId, notifierName);
            }
        }
    }

    displayUsers(users) {
        const tbody = document.getElementById('usersTableBody');
        const emptyState = document.getElementById('usersEmptyState');

        if (users.length === 0) {
            tbody.innerHTML = '';
            emptyState.style.display = 'block';
            return;
        }

        emptyState.style.display = 'none';
        tbody.innerHTML = users.map(user => `
            <tr>
                <td>${user.userNumber}</td>
                <td>${user.userName}</td>
                <td>
                    <span class="badge ${user.notices && user.notices.length > 0 ? 'badge-success' : 'badge-warning'}">
                        ${user.notices && user.notices.length > 0 ? 'Notified' : 'Pending'}
                    </span>
                </td>
            </tr>
        `).join('');
    }

    displayNotices(notices) {
        const tbody = document.getElementById('noticesTableBody');
        const emptyState = document.getElementById('noticesEmptyState');

        if (notices.length === 0) {
            tbody.innerHTML = '';
            emptyState.style.display = 'block';
            return;
        }

        emptyState.style.display = 'none';
        tbody.innerHTML = notices.map(notice => `
            <tr>
                <td>
                    <div class="notification-title">${notice.notifierName}</div>
                </td>
                <td>
                    <div class="notification-message">${notice.message}</div>
                </td>
                <td>
                    <button class="btn btn-success" onclick="app.notifyUsers('${notice.id}', '${notice.notifierName}')">
                        <i class="fas fa-paper-plane"></i> Notify Users
                    </button>
                </td>
            </tr>
        `).join('');
    }

    updateStats() {
        // This would be enhanced with actual API calls for detailed stats
        const userRows = document.querySelectorAll('#usersTableBody tr');
        const noticeRows = document.querySelectorAll('#noticesTableBody tr');

        const notifiedCount = Array.from(userRows).filter(row =>
            row.querySelector('.badge-success')
        ).length;

        document.getElementById('totalUsers').textContent = userRows.length;
        document.getElementById('totalNotices').textContent = noticeRows.length;
        document.getElementById('notifiedUsers').textContent = notifiedCount;
        document.getElementById('pendingUsers').textContent = userRows.length - notifiedCount;
    }

    setButtonLoading(button, isLoading) {
        if (isLoading) {
            button.disabled = true;
            button.innerHTML = `<div class="loading"></div> Loading...`;
        } else {
            button.disabled = false;
            const originalText = button.getAttribute('data-original-text');
            if (originalText) {
                button.innerHTML = originalText;
            }
        }
    }

    showToast(message, type = 'success') {
        const toastContainer = document.getElementById('toastContainer');
        const toast = document.createElement('div');
        toast.className = `toast ${type}`;
        toast.innerHTML = `
            <i class="fas fa-${this.getToastIcon(type)}"></i>
            ${message}
        `;

        toastContainer.appendChild(toast);

        setTimeout(() => toast.classList.add('show'), 100);

        setTimeout(() => {
            toast.classList.remove('show');
            setTimeout(() => toast.remove(), 300);
        }, 4000);
    }

    getToastIcon(type) {
        const icons = {
            success: 'check-circle',
            error: 'exclamation-circle',
            warning: 'exclamation-triangle'
        };
        return icons[type] || 'info-circle';
    }

    formatFileSize(bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
    }
}

// Store original button texts
document.addEventListener('DOMContentLoaded', () => {
    const buttons = document.querySelectorAll('.btn');
    buttons.forEach(btn => {
        btn.setAttribute('data-original-text', btn.innerHTML);
    });

    // Initialize the application
    window.app = new NotificationSystem();
});
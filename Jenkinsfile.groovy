pipeline {
    agent any
    
    environment {
        SOLUTION_FILE = 'Last Api.sln'
        SONAR_TOKEN = credentials('SONAR_TOKEN')
        PATH = "C:\\Windows\\system32\\config\\systemprofile\\.dotnet\\tools;${env.PATH}"
    }
    
    stages {
        stage('Checkout') {
            steps {
                git branch: 'main', url: 'https://github.com/CallumCompSci/API'
            }
        }
        stage('Install Dependencies') {
            steps {
                bat """
                dotnet restore \"${SOLUTION_FILE}\"
                dotnet tool install --global dotnet-sonarscanner || dotnet tool update --global dotnet-sonarscanner
                npm install -g newman
                """
            }
        }
        
        stage('Build') {
            steps {
                bat 'dotnet clean "Last Api.sln" --configuration Release'
                bat 'dotnet build "Last Api.sln" --configuration Release --no-restore'
            }
        }
        stage('Run Tests') {
                steps {
                    bat """
                    dotnet test \"${SOLUTION_FILE}\"  --configuration Release --logger trx --results-directory ./TestResults --verbosity normal
                    """
                }
        }
        stage('SonarQube Analysis') {
            steps {
                bat """
                dotnet-sonarscanner begin ^
                    /k:"KEY" ^
                    /o:"ACCOUNT" ^
                    /d:sonar.host.url="https://sonarcloud.io" ^
                    /d:sonar.token="%SONAR_TOKEN%"
                    /d:sonar.exclusions="**/node_modules/**,**/bin/**,**/obj/**,**/Testing/**"
                
                dotnet build "${SOLUTION_FILE}" --configuration Release --no-restore
                
                dotnet test "${SOLUTION_FILE}" --configuration Release --logger trx --results-directory ./TestResults
                
                dotnet-sonarscanner end /d:sonar.token="%SONAR_TOKEN%"
                """
            }
        } 
        stage('Security Scan') {
            steps {
                bat '''
                    dotnet list package --vulnerable --include-transitive > security-report.txt
                    type security-report.txt
                '''
            }
        }
        stage('Deploy') {
            steps {
                bat 'docker build -t last-api:test .'
                bat '''
                docker stop last-api-test || true
                docker rm last-api-test || true
                docker run -d -p 5000:80 --name last-api-test last-api:test
                '''
            }
        }

    }
}

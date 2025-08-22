pipeline {
    agent { label 'amazon-linux' }
    
    environment {
        // AWS credentials should be configured in Jenkins credentials
        AWS_DEFAULT_REGION = 'eu-west-2'
        DOCKER_COMPOSE_FILE = "${WORKSPACE}/tmp2-original/docker-compose.yml"
        RESULTS_DIR = "${WORKSPACE}/test-results"
        PROJECT_NAME = 'UI-tests-maui'
    }
    
    stages {
        stage('Checkout') {
            steps {
                echo 'Checking out code from repository...'
                checkout scm
            }
        }
        
        stage('Launch Genymotion EC2') {
            steps {
                script {
                    echo 'Launching Genymotion EC2 instance...'
                    
                    // Make the script executable and run it
                    sh '''
                        chmod +x ${WORKSPACE}/genymotion_ec2_runner.sh
                        
                        # Run the script to create EC2 instance
                        ${WORKSPACE}/genymotion_ec2_runner.sh
                    '''
                }
            }
        }
        
        stage('Load Docker Images') {
            steps {
                script {
                    echo 'Downloading Docker image tarballs from S3 and loading...'
                    sh '''
                        set -euo pipefail
                        S3_PATH="s3://ads-jenkins-s3-staging/source-code"

                        # Ensure destination directory exists
                        mkdir -p ${WORKSPACE}/tmp2-original/docker-images

                        echo "Downloading images from ${S3_PATH} ..."
                        aws s3 cp ${S3_PATH}/vm-app.tar.gz ${WORKSPACE}/tmp2-original/docker-images/vm-app.tar.gz --no-progress
                        aws s3 cp ${S3_PATH}/vm-vs-sync-app.tar.gz ${WORKSPACE}/tmp2-original/docker-images/vm-vs-sync-app.tar.gz --no-progress
                        aws s3 cp ${S3_PATH}/vm-tt-app.tar.gz ${WORKSPACE}/tmp2-original/docker-images/vm-tt-app.tar.gz --no-progress

                        # Verify downloads are present and non-empty
                        for name in vm-app vm-vs-sync-app vm-tt-app; do
                            file="${WORKSPACE}/tmp2-original/docker-images/${name}.tar.gz"
                            if [ ! -s "$file" ]; then
                                echo "Error: download failed or file empty: $file"
                                exit 1
                            fi
                        done
                        echo "All Docker image tarballs downloaded successfully."

                        # Change to tmp2-original directory and run load-images.sh from there
                        cd ${WORKSPACE}/tmp2-original
                        chmod +x load-images.sh
                        
                        # Verify the script can see the docker-images directory
                        echo "Current directory: $(pwd)"
                        echo "Contents of current directory:"
                        ls -la
                        echo "Contents of docker-images directory:"
                        ls -la docker-images/
                        
                        ./load-images.sh
                    '''
                }
            }
        }
        
        stage('Setup Environment') {
            steps {
                script {
                    echo 'Setting up environment files from Jenkins credentials...'
                    
                    // Copy environment files from Jenkins credentials
                    withCredentials([
                        file(credentialsId: 'env-file', variable: 'ENV_FILE'),
                        file(credentialsId: 'ttg-env-file', variable: 'TTG_ENV_FILE')
                    ]) {
                        sh '''
                            # Ensure the tmp2-original directory exists
                            mkdir -p ${WORKSPACE}/tmp2-original
                            
                            # Copy environment files to workspace
                            cp "$ENV_FILE" ${WORKSPACE}/tmp2-original/.env
                            cp "$TTG_ENV_FILE" ${WORKSPACE}/tmp2-original/ttg.env
                            
                            # Set proper permissions
                            chmod 600 ${WORKSPACE}/tmp2-original/.env
                            chmod 600 ${WORKSPACE}/tmp2-original/ttg.env
                            
                            # Verify files were copied
                            echo "Environment files copied successfully:"
                            ls -la ${WORKSPACE}/tmp2-original/.env ${WORKSPACE}/tmp2-original/ttg.env
                            
                            echo "Environment files setup completed"
                        '''
                    }
                }
            }
        }
        
        stage('Run Tests') {
            steps {
                script {
                    echo 'Running UI tests with docker-compose...'
                    
                    // Get the EC2 instance ID from the script output or environment
                    sh '''
                        # Create results directory
                        mkdir -p ${RESULTS_DIR}
                        
                        # Ensure we're in the right directory and verify files exist
                        cd ${WORKSPACE}/tmp2-original
                        echo "Current directory: $(pwd)"
                        echo "Checking for required files:"
                        ls -la
                        echo "Checking environment files:"
                        if [ -f ".env" ]; then
                            echo ".env found"
                        else
                            echo "ERROR: .env file not found!"
                            exit 1
                        fi
                        if [ -f "ttg.env" ]; then
                            echo "ttg.env found"
                        else
                            echo "ERROR: ttg.env file not found!"
                            exit 1
                        fi
                        
                        # Run docker-compose for tests (will use .env and ttg.env files)
                        docker-compose up -d
                        
                        # Wait for tests to complete (up to 600 seconds or until log message appears)
                        echo "Waiting for 'UI tests completed' message in test-runner logs (timeout: 600s)..."
                        end=$((SECONDS+600))
                        found=0
                        while [ $SECONDS -lt $end ]; do
                          if docker-compose logs test-runner | grep -q "UI tests completed"; then
                            found=1
                            echo "Detected 'UI tests completed' in logs."
                            break
                          fi
                          sleep 5
                        done
                        if [ $found -eq 0 ]; then
                          echo "Timeout reached (600s) without detecting 'UI tests completed' in logs."
                        fi
                        
                        # Stop containers
                        docker-compose down
                    '''
                }
            }
        }
        
        stage('Collect Results') {
            steps {
                script {
                    echo 'Collecting test results...'
                    
                    sh '''
                        # Create results directory if it doesn't exist
                        mkdir -p ${RESULTS_DIR}
                        
                        # Copy test results from docker containers
                        cd ${WORKSPACE}/tmp2-original
                        
                        # Copy test output files from the mounted volume
                        if [ -d "output" ]; then
                            echo "Copying test results from output directory..."
                            cp -r output/* ${RESULTS_DIR}/ || true
                            echo "Test results copied to ${RESULTS_DIR}"
                            ls -la ${RESULTS_DIR}/
                        else
                            echo "Warning: output directory not found"
                        fi
                        
                        # Copy docker logs
                        echo "Collecting docker logs..."
                        docker-compose logs > ${RESULTS_DIR}/docker-logs.txt 2>&1 || true
                        
                        # Copy any other relevant test files
                        echo "Looking for additional test files..."
                        find . -name "*.xml" -o -name "*.html" -o -name "*.json" -o -name "*.trx" -o -name "*.mp4" | head -20 | xargs -I {} cp {} ${RESULTS_DIR}/ || true
                        
                        # Show what was collected
                        echo "Collected files in ${RESULTS_DIR}:"
                        ls -la ${RESULTS_DIR}/ || true
                    '''
                    // Upload to S3 with date + build number inside 'results/' folder
                    sh '''
                        UPLOAD_DATE=$(date +%Y-%m-%d)
                        DEST_PATH="results/${PROJECT_NAME}/${UPLOAD_DATE}/build-${BUILD_NUMBER}"

                        echo "Uploading test results to S3 path: ${DEST_PATH}"
                        aws s3 cp ${RESULTS_DIR} s3://ads-jenkins-s3-staging/${DEST_PATH}/ --recursive
                        echo "Test results uploaded to s3://ads-jenkins-s3-staging/${DEST_PATH}/"
                    '''
                }
            }
        }
    }
    
post {
    always {
        script {
            echo 'Publishing test results...'
            
            // Archive test results
            archiveArtifacts artifacts: 'test-results/**/*', fingerprint: true
            
            // Publish test results (if you have JUnit XML reports)
            sh '''
                if [ -d "${RESULTS_DIR}" ]; then
                    echo "Test results available in: ${RESULTS_DIR}"
                    ls -la ${RESULTS_DIR}/
                fi
            '''
        }
    }
    
    success {
        echo 'Pipeline completed successfully!'
    }
    
    failure {
        echo 'Pipeline failed!'
    }
    
    // cleanup {
    //     script {
    //         echo 'Cleaning up resources...'
            
    //         sh '''
    //             set +e  # Don't exit on errors during cleanup
    //             echo "Cleaning up Genymotion EC2 instance..."
    //             # Get the instance ID from the script output or a file
    //             if [ -f "${WORKSPACE}/instance_id.txt" ]; then
    //                 INSTANCE_ID=$(cat "${WORKSPACE}/instance_id.txt")
    //                 if [ -n "$INSTANCE_ID" ]; then
    //                     echo "Terminating EC2 instance: $INSTANCE_ID"
    //                     aws ec2 terminate-instances --instance-ids "$INSTANCE_ID" --region eu-west-2 || echo "Warning: Failed to terminate EC2 instance"
    //                 fi
    //             else
    //                 echo "No instance_id.txt found, skipping EC2 cleanup"
    //             fi
                
    //             # Clean up docker resources
    //             if [ -d "${WORKSPACE}/tmp2-original" ]; then
    //                 cd "${WORKSPACE}/tmp2-original"
    //                 echo "Cleaning up Docker resources..."
    //                 docker-compose down -v || echo "Warning: Failed to stop docker-compose"
    //                 docker system prune -f || echo "Warning: Failed to prune Docker system"
    //             else
    //                 echo "tmp2-original directory not found, skipping Docker cleanup"
    //             fi
                
    //             # Additional cleanup for any remaining containers/images
    //             docker container prune -f || echo "Warning: Failed to prune containers"
    //             docker image prune -f || echo "Warning: Failed to prune images"
    //             docker volume prune -f || echo "Warning: Failed to prune volumes"
    //         '''
            
    //         echo 'Cleaning up workspace...'
    //         cleanWs()
    //     }
    // }
}
}

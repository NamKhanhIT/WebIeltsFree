# Quick Reference: Phase 4 & 5 Commands

## Phase 4: Database Seeding

### Option 1: PowerShell (Windows)
```powershell
cd DatabaseScripts
.\run_seeds.ps1 -Host localhost -Port 3307 -User root -Password root
```

### Option 2: Bash (Linux/Mac)
```bash
cd DatabaseScripts
chmod +x run_seeds.sh
./run_seeds.sh
```

### Option 3: Manual MySQL
```bash
mysql -h localhost -P 3307 -u root -p ieltsdb < ieltsdb_v2_migration_safe.sql
mysql -h localhost -P 3307 -u root -p ieltsdb < ieltsdb_v2_seed_data.sql
mysql -h localhost -P 3307 -u root -p ieltsdb < seed_lessons_content.sql
```

### Verify Seeding
```sql
mysql -h localhost -P 3307 -u root -p ieltsdb -e "
SELECT 'Courses' as entity, COUNT(*) as count FROM tb_courses
UNION ALL SELECT 'Modules', COUNT(*) FROM tb_modules
UNION ALL SELECT 'Lessons', COUNT(*) FROM tb_lessons
UNION ALL SELECT 'Speaking Topics', COUNT(*) FROM tb_speaking_topics
UNION ALL SELECT 'Writing Prompts', COUNT(*) FROM tb_writing_prompts
UNION ALL SELECT 'Vocabulary', COUNT(*) FROM tb_vocabulary
UNION ALL SELECT 'Tests', COUNT(*) FROM tb_tests;
"
```

---

## Phase 5: Docker Deployment

### Development Environment

```bash
# Start services
docker-compose -f docker-compose.dev.yml up -d

# Check status
docker-compose -f docker-compose.dev.yml ps

# View logs
docker-compose -f docker-compose.dev.yml logs -f web

# Access web app
curl http://localhost:5000/health

# Access services
# phpMyAdmin: http://localhost:8081
# Mongo Express: http://localhost:8082

# Stop services
docker-compose -f docker-compose.dev.yml down

# Clean up everything
docker-compose -f docker-compose.dev.yml down -v
docker system prune -a
```

### Production Environment

```bash
# Setup environment
cp .env.prod.template .env
# Edit .env with production values

# Start production stack
docker-compose -f docker-compose.prod.yml up -d

# Check health
docker-compose -f docker-compose.prod.yml ps
docker-compose -f docker-compose.prod.yml logs -f

# Access services
# Web App: https://yourdomain.com (via nginx)
# Grafana: https://yourdomain.com:3000
# Prometheus: https://yourdomain.com:9090

# Stop services
docker-compose -f docker-compose.prod.yml down
```

### Docker Troubleshooting

```bash
# View container logs
docker logs container-name
docker logs container-name --tail 100

# Execute command in container
docker exec -it container-name bash

# Inspect container
docker inspect container-name

# Check resource usage
docker stats

# Clean unused resources
docker system prune -a
```

---

## Phase 5: Kubernetes Deployment

### Prerequisites

```bash
# Check kubectl installed
kubectl version --client

# Check connection to cluster
kubectl cluster-info
kubectl get nodes
```

### Deploy to Kubernetes

```bash
# Create all resources
kubectl apply -f kubernetes-manifest.yml

# Verify resources created
kubectl get all -n ielts

# Check pods
kubectl get pods -n ielts
kubectl get pods -n ielts -w

# Check services
kubectl get svc -n ielts
```

### Monitor Deployment

```bash
# View pod logs
kubectl logs -n ielts deployment/ielts-web

# View live logs
kubectl logs -n ielts deployment/ielts-web -f

# Describe pod
kubectl describe pod -n ielts <pod-name>

# Check events
kubectl get events -n ielts

# View resource usage
kubectl top nodes
kubectl top pods -n ielts
```

### Scale and Update

```bash
# Manual scaling
kubectl scale deployment ielts-web -n ielts --replicas=5

# View HPA status
kubectl get hpa -n ielts
kubectl describe hpa ielts-web-hpa -n ielts

# Update image
kubectl set image deployment/ielts-web web=yourusername/ielts-web:v2 -n ielts

# Monitor rollout
kubectl rollout status deployment/ielts-web -n ielts

# Rollback if needed
kubectl rollout undo deployment/ielts-web -n ielts
```

### Access Application

```bash
# Port forward
kubectl port-forward -n ielts svc/ielts-web 8080:80

# Access: http://localhost:8080
curl http://localhost:8080/health

# Get LoadBalancer IP
kubectl get svc ielts-web -n ielts -o jsonpath='{.status.loadBalancer.ingress[0].ip}'
```

### Troubleshooting

```bash
# View pod status details
kubectl describe pod -n ielts <pod-name>

# Check pod logs
kubectl logs -n ielts <pod-name>

# Debug interactive shell
kubectl debug -n ielts <pod-name> -it

# Check DNS
kubectl run -it --image=busybox dns-test -n ielts -- nslookup mysql

# View config map
kubectl get configmap -n ielts -o yaml

# View secrets (base64 encoded)
kubectl get secret -n ielts -o yaml
```

### Cleanup

```bash
# Delete namespace (removes everything)
kubectl delete namespace ielts

# Delete specific resource
kubectl delete deployment ielts-web -n ielts
```

---

## Phase 5: CI/CD Pipeline

### Setup

```bash
# Copy workflow file
mkdir -p .github/workflows
cp github-actions-ci-cd.yml .github/workflows/ci-cd.yml

# Add secrets
gh secret set KUBE_CONFIG --body "$(base64 ~/.kube/config)"
gh secret set SONAR_TOKEN --body "your-sonarcloud-token"

# Commit and push
git add .github/
git commit -m "Add CI/CD pipeline"
git push origin main
```

### Monitor Workflows

```bash
# View workflow runs
gh run list

# View specific run logs
gh run view <run-id> --log

# Re-run workflow
gh run rerun <run-id>

# Cancel running workflow
gh run cancel <run-id>

# View via web
https://github.com/your-username/WebIeltsFree/actions
```

### Troubleshooting CI/CD

```bash
# Check workflow syntax
gh workflow view ci-cd.yml

# View secrets
gh secret list

# Create/update secrets
gh secret set SECRET_NAME --body "value"

# View environment variables
gh environment list
```

---

## Database Connection Strings

### Development
```
MySQL: mysql -h localhost -P 3307 -u root -p root
MongoDB: mongodb://admin:admin_password@localhost:27017
Redis: redis://:redis_password@localhost:6379
```

### Docker Compose
```
MySQL: mysql -h mysql -P 3306 -u root -p root
MongoDB: mongodb://admin:admin_password@mongodb:27017
Redis: redis://:redis_password@redis:6379
```

### Kubernetes
```
MySQL: mysql -h mysql -P 3306 -u ielts_user -p <password>
MongoDB: mongodb://admin:<password>@mongodb:27017
Redis: redis://:password@redis:6379
```

---

## Useful Endpoints

### Development
- Web App Health: http://localhost:5000/health
- phpMyAdmin: http://localhost:8081
- Mongo Express: http://localhost:8082
- API: http://localhost:5000

### Production (with Nginx)
- Web App: https://yourdomain.com
- API: https://api.yourdomain.com
- Grafana: https://yourdomain.com:3000
- Prometheus: https://yourdomain.com:9090

---

## Emergency Commands

### Database Backup
```bash
# MySQL
mysqldump -h localhost -P 3307 -u root -p root ieltsdb > backup.sql

# MongoDB
mongodump --uri="mongodb://admin:password@localhost:27017" --out=./backup

# Restore MySQL
mysql -h localhost -P 3307 -u root -p root ieltsdb < backup.sql
```

### Docker Emergency
```bash
# Stop all containers
docker stop $(docker ps -q)

# Remove all containers
docker rm $(docker ps -aq)

# Remove all volumes
docker volume rm $(docker volume ls -q)

# Full cleanup
docker system prune -a -v
```

### Kubernetes Emergency
```bash
# Delete entire namespace
kubectl delete namespace ielts

# Force delete stuck pod
kubectl delete pod <pod-name> -n ielts --grace-period=0 --force

# Restart deployment
kubectl rollout restart deployment/ielts-web -n ielts

# Emergency cleanup
kubectl delete all --all -n ielts
```

---

## Performance Tuning

### MySQL
```sql
-- Check query performance
SHOW PROCESSLIST;
EXPLAIN SELECT * FROM tb_courses;

-- Optimize tables
OPTIMIZE TABLE tb_courses, tb_modules, tb_lessons;

-- Check indexes
SHOW INDEX FROM tb_courses;
```

### Redis
```bash
# Check memory usage
redis-cli INFO memory

# Check connected clients
redis-cli INFO clients

# Clear cache
redis-cli FLUSHDB
```

### MongoDB
```bash
# Check server status
mongosh --eval "db.serverStatus()"

# Check collection stats
mongosh --eval "db.stats()"

# Clear database
mongosh --eval "db.dropDatabase()"
```

---

## Documentation Files

Quick Links:
- Implementation Guide: PHASE4_5_IMPLEMENTATION_GUIDE.md
- Summary: PHASE4_5_SUMMARY.md
- Checklist: PHASE4_5_CHECKLIST.md
- This Quick Reference: QUICK_REFERENCE.md

---

Last Updated: 2026-04-09
